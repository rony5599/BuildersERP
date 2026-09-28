using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.Common.Caching;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CashDisbursements;

public enum CreateCashDisbursementResult
{
    Success,
    RequisitionNotDisbursable
}

public record CreateCashDisbursementCommand(CreateCashDisbursementDto Dto) : IRequest<CreateCashDisbursementResult>, IInvalidatesFeatures
{
    // Disbursements are the debits of the requester ledger, which is cached under RequesterLedger.
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["RequesterLedger"];
}

public class CreateCashDisbursementCommandHandler : IRequestHandler<CreateCashDisbursementCommand, CreateCashDisbursementResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreateCashDisbursementCommandHandler(IUnitOfWork unitOfWork, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _numberGenerator = numberGenerator;
    }

    public async Task<CreateCashDisbursementResult> Handle(CreateCashDisbursementCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var requisition = await _unitOfWork.Repository<CashRequisition>().Query()
            .Where(r => r.Id == dto.CashRequisitionId)
            .Select(r => new { r.IsActive, r.Status, r.RequesterEmployeeId, r.ProjectId })
            .FirstOrDefaultAsync(cancellationToken);

        if (requisition is null || !requisition.IsActive || !DisbursableRequisitionsHandler.Statuses.Contains(requisition.Status))
        {
            return CreateCashDisbursementResult.RequisitionNotDisbursable;
        }

        var disbursement = new CashDisbursement
        {
            DisbursementNumber = await _numberGenerator.GenerateAsync(requisition.ProjectId, "CDSB", cancellationToken),
            DisbursementDate = dto.DisbursementDate,
            Amount = dto.Amount,
            Method = dto.Method,
            ReferenceNumber = dto.ReferenceNumber,
            Remarks = dto.Remarks,
            CashRequisitionId = dto.CashRequisitionId,
            RequesterEmployeeId = requisition.RequesterEmployeeId
        };

        await _unitOfWork.Repository<CashDisbursement>().AddAsync(disbursement);
        await _unitOfWork.SaveChangesAsync();
        return CreateCashDisbursementResult.Success;
    }
}

public record SetCashDisbursementActiveCommand(long Id, bool IsActive) : IRequest<bool>, IInvalidatesFeatures
{
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["RequesterLedger"];
}

public class SetCashDisbursementActiveCommandHandler : IRequestHandler<SetCashDisbursementActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetCashDisbursementActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetCashDisbursementActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CashDisbursement>();
        var item = await repository.GetByIdAsync(request.Id);
        if (item is null)
        {
            return false;
        }

        item.IsActive = request.IsActive;
        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}

public record GetAllCashDisbursementsQuery(
    int Page = 1,
    int PageSize = 25,
    string? Search = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<CashDisbursementDto>>;

public class GetAllCashDisbursementsQueryHandler : IRequestHandler<GetAllCashDisbursementsQuery, PagedResult<CashDisbursementDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCashDisbursementsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<CashDisbursementDto>> Handle(GetAllCashDisbursementsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<CashDisbursement>().Query()
            .Include(d => d.CashRequisition)
            .Include(d => d.RequesterEmployee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(d => d.DisbursementNumber.Contains(term)
                                     || d.CashRequisition.RequisitionNumber.Contains(term)
                                     || d.RequesterEmployee.EmployeeName.Contains(term));
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(d => d.DisbursementDate >= request.DateFrom.Value.Date);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(d => d.DisbursementDate < request.DateTo.Value.Date.AddDays(1));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(d => d.DisbursementDate).ThenByDescending(d => d.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<CashDisbursementDto>(_mapper.Map<IReadOnlyList<CashDisbursementDto>>(items), totalCount, page, pageSize);
    }
}

public record GetDisbursableRequisitionsQuery : IRequest<IReadOnlyList<DisbursableRequisitionDto>>;

public class DisbursableRequisitionsHandler : IRequestHandler<GetDisbursableRequisitionsQuery, IReadOnlyList<DisbursableRequisitionDto>>
{
    public static readonly RequisitionStatus[] Statuses = { RequisitionStatus.Approved, RequisitionStatus.Converted };

    private readonly IUnitOfWork _unitOfWork;

    public DisbursableRequisitionsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<DisbursableRequisitionDto>> Handle(GetDisbursableRequisitionsQuery request, CancellationToken cancellationToken)
    {
        var statuses = Statuses;
        var requisitions = await _unitOfWork.Repository<CashRequisition>().Query()
            .Where(r => r.IsActive && statuses.Contains(r.Status))
            .OrderByDescending(r => r.RequestDate)
            .Select(r => new DisbursableRequisitionDto
            {
                Id = r.Id,
                RequisitionNumber = r.RequisitionNumber,
                RequesterName = r.RequesterEmployee.EmployeeName,
                EstimatedAmount = r.EstimatedAmount
            })
            .ToListAsync(cancellationToken);

        var disbursed = await _unitOfWork.Repository<CashDisbursement>().Query()
            .Where(d => d.IsActive)
            .GroupBy(d => d.CashRequisitionId)
            .Select(g => new { g.Key, Amount = g.Sum(x => x.Amount) })
            .ToDictionaryAsync(x => x.Key, x => x.Amount, cancellationToken);

        foreach (var r in requisitions)
        {
            disbursed.TryGetValue(r.Id, out var amount);
            r.DisbursedAmount = amount;
        }

        return requisitions;
    }
}
