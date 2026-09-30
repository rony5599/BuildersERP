using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.EngineerWorkOrders;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EwoBills;

internal static class EwoBillPayments
{
    public static async Task<Dictionary<long, decimal>> PaidByBillAsync(IUnitOfWork unitOfWork, IReadOnlyCollection<long> billIds, CancellationToken ct)
    {
        return await unitOfWork.Repository<SupplierPayment>().Query()
            .Where(p => p.IsActive && p.EwoBillId != null && billIds.Contains(p.EwoBillId.Value))
            .GroupBy(p => p.EwoBillId!.Value)
            .Select(g => new { g.Key, Amount = g.Sum(x => x.Amount) })
            .ToDictionaryAsync(x => x.Key, x => x.Amount, ct);
    }

    public static async Task FillPaidAsync(IUnitOfWork unitOfWork, IReadOnlyCollection<EwoBillDto> bills, CancellationToken ct)
    {
        var paid = await PaidByBillAsync(unitOfWork, bills.Select(b => b.Id).ToList(), ct);
        foreach (var bill in bills)
        {
            paid.TryGetValue(bill.Id, out var amount);
            bill.PaidAmount = amount;
        }
    }
}

public record GetAllEwoBillsQuery(
    int Page = 1,
    int PageSize = 25,
    string? BillNumber = null,
    string? Supplier = null,
    PoBillStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<EwoBillDto>>;

public class GetAllEwoBillsQueryHandler : IRequestHandler<GetAllEwoBillsQuery, PagedResult<EwoBillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllEwoBillsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<EwoBillDto>> Handle(GetAllEwoBillsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<EwoBill>().Query()
            .Include(b => b.EngineerWorkOrder).ThenInclude(o => o.EngineerWorkOrderRequisition).ThenInclude(r => r.Project)
            .Include(b => b.Supplier)
            .Include(b => b.Heads)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.BillNumber))
        {
            var term = request.BillNumber.Trim();
            query = query.Where(b => b.BillNumber.Contains(term) || b.EngineerWorkOrder.WorkOrderNo.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.Supplier))
        {
            var term = request.Supplier.Trim();
            query = query.Where(b => b.Supplier.Name.Contains(term));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(b => b.Status == request.Status.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(b => b.BillDate >= request.DateFrom.Value.Date);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(b => b.BillDate < request.DateTo.Value.Date.AddDays(1));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var bills = await query
            .OrderByDescending(b => b.BillDate).ThenByDescending(b => b.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<List<EwoBillDto>>(bills);
        await EwoBillPayments.FillPaidAsync(_unitOfWork, items, cancellationToken);
        return new PagedResult<EwoBillDto>(items, totalCount, page, pageSize);
    }
}

public record GetEwoBillByIdQuery(long Id) : IRequest<EwoBillDto?>;

public class GetEwoBillByIdQueryHandler : IRequestHandler<GetEwoBillByIdQuery, EwoBillDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEwoBillByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EwoBillDto?> Handle(GetEwoBillByIdQuery request, CancellationToken cancellationToken)
    {
        var bill = await _unitOfWork.Repository<EwoBill>().Query()
            .Include(b => b.EngineerWorkOrder).ThenInclude(o => o.EngineerWorkOrderRequisition).ThenInclude(r => r.Project)
            .Include(b => b.Supplier)
            .Include(b => b.Details).ThenInclude(d => d.Material)
            .Include(b => b.Heads)
            .Include(b => b.Adjustments)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        if (bill is null)
        {
            return null;
        }

        var dto = _mapper.Map<EwoBillDto>(bill);
        await EwoBillPayments.FillPaidAsync(_unitOfWork, new[] { dto }, cancellationToken);
        return dto;
    }
}

public record GetEwoBillPrintDataQuery(long Id) : IRequest<EwoBillPrintDto?>;

public class GetEwoBillPrintDataQueryHandler : IRequestHandler<GetEwoBillPrintDataQuery, EwoBillPrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEwoBillPrintDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EwoBillPrintDto?> Handle(GetEwoBillPrintDataQuery request, CancellationToken cancellationToken)
    {
        var bill = await _unitOfWork.Repository<EwoBill>().Query()
            .Include(b => b.EngineerWorkOrder).ThenInclude(o => o.EngineerWorkOrderRequisition).ThenInclude(r => r.Project)
            .Include(b => b.Supplier)
            .Include(b => b.Details).ThenInclude(d => d.Material)
            .Include(b => b.Heads)
            .Include(b => b.Adjustments)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (bill is null)
        {
            return null;
        }

        var company = await _unitOfWork.Repository<Company>().Query()
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        var sequence = await _unitOfWork.Repository<EwoBill>().Query()
            .CountAsync(b => b.RootWorkOrderId == bill.RootWorkOrderId && b.Id <= bill.Id
                             && b.IsActive && b.Status != PoBillStatus.Cancelled, cancellationToken);

        var supplier = bill.Supplier;
        return new EwoBillPrintDto
        {
            BillNumber = bill.BillNumber,
            BillDate = bill.BillDate,
            ContractorBillNumber = bill.ContractorBillNumber,
            MrrNumber = bill.MrrNumber,
            WorkOrderNo = bill.EngineerWorkOrder.WorkOrderNo,
            ProjectName = bill.EngineerWorkOrder.EngineerWorkOrderRequisition?.Project?.Name,
            BillSequence = sequence,
            Status = bill.Status,
            Remarks = bill.Remarks,
            PreparedBy = bill.CreatedBy,
            CompanyName = company?.Name ?? string.Empty,
            CompanyAddress = company?.Address,
            CompanyPhone = company?.Phone,
            CompanyEmail = company?.Email,
            SupplierName = supplier?.Name ?? string.Empty,
            SupplierAddress = supplier?.Address,
            SupplierPhone = supplier?.Phone,
            SupplierEmail = supplier?.Email,
            Lines = bill.Details.OrderBy(d => d.EngineerWorkOrderDetailId).Select(d => new EngineerWorkOrderPrintLineDto
            {
                Description = d.Material?.Name ?? string.Empty,
                UnitOfMeasure = d.UnitOfMeasure.ToString(),
                Quantity = d.MeasuredQuantity,
                Rate = d.Rate,
                Amount = d.Amount
            }).ToList(),
            Heads = bill.Heads.Select(h => new EwoBillHeadDto
            {
                EngineerWorkOrderPaymentHeadId = h.EngineerWorkOrderPaymentHeadId,
                HeadName = h.HeadName,
                HeadPercent = h.HeadPercent,
                ClaimPercent = h.ClaimPercent
            }).ToList(),
            Adjustments = bill.Adjustments.Select(a => new EwoBillAdjustmentDto
            {
                Type = a.Type,
                Description = a.Description,
                Amount = a.Amount
            }).ToList(),
            MeasuredAmount = bill.MeasuredAmount,
            CumulativePercent = bill.CumulativePercent,
            CumulativeDue = bill.CumulativeDue,
            PreviouslyCertified = bill.PreviouslyCertified,
            CertifiedAmount = bill.CertifiedAmount,
            AdditionAmount = bill.AdditionAmount,
            DeductionAmount = bill.DeductionAmount,
            NetPayable = bill.NetPayable
        };
    }
}

public record GetBillableEngineerWorkOrdersQuery(long? IncludeWorkOrderId = null) : IRequest<IReadOnlyList<BillableEngineerWorkOrderDto>>;

public class GetBillableEngineerWorkOrdersQueryHandler : IRequestHandler<GetBillableEngineerWorkOrdersQuery, IReadOnlyList<BillableEngineerWorkOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBillableEngineerWorkOrdersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<BillableEngineerWorkOrderDto>> Handle(GetBillableEngineerWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var statuses = EwoBillBuilder.BillableStatuses;
        return await _unitOfWork.Repository<EngineerWorkOrder>().Query()
            .Where(o => (o.IsLatestRevision && statuses.Contains(o.Status) && o.PaymentHeads.Any()) || o.Id == request.IncludeWorkOrderId)
            .OrderByDescending(o => o.Id)
            .Select(o => new BillableEngineerWorkOrderDto
            {
                Id = o.Id,
                WorkOrderNo = o.WorkOrderNo,
                SupplierName = o.Supplier.Name,
                ProjectName = o.EngineerWorkOrderRequisition.Project.Name
            })
            .ToListAsync(cancellationToken);
    }
}

public record GetEwoBillableDataQuery(long EngineerWorkOrderId, long? ExcludeBillId = null) : IRequest<EwoBillableDataDto?>;

public class GetEwoBillableDataQueryHandler : IRequestHandler<GetEwoBillableDataQuery, EwoBillableDataDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEwoBillableDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EwoBillableDataDto?> Handle(GetEwoBillableDataQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<EngineerWorkOrder>().Query()
            .Include(o => o.Supplier)
            .Include(o => o.Details).ThenInclude(d => d.Material)
            .Include(o => o.PaymentHeads)
            .FirstOrDefaultAsync(o => o.Id == request.EngineerWorkOrderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        var prior = await EwoBillBuilder.LoadPriorBillsAsync(_unitOfWork, order.MotherWorkOrderId ?? order.Id, request.ExcludeBillId, cancellationToken);

        // Measurement is cumulative, so a new bill starts from the latest earlier measurement.
        // Lines are matched by work order line, or by material when the bill was on an earlier revision.
        var lastBill = prior.Bills.LastOrDefault();
        decimal PreviousQty(EngineerWorkOrderDetail line)
        {
            if (lastBill is null)
            {
                return 0;
            }

            var exact = lastBill.Details.Where(d => d.EngineerWorkOrderDetailId == line.Id).ToList();
            return exact.Count > 0
                ? exact.Sum(d => d.MeasuredQuantity)
                : lastBill.EngineerWorkOrderId == order.Id ? 0 : lastBill.Details.Where(d => d.MaterialId == line.MaterialId).Sum(d => d.MeasuredQuantity);
        }

        return new EwoBillableDataDto
        {
            EngineerWorkOrderId = order.Id,
            WorkOrderNo = order.WorkOrderNo,
            SupplierName = order.Supplier?.Name ?? string.Empty,
            ContractAmount = order.TotalAmount,
            PreviousCumulativePercent = prior.CumulativePercent,
            PreviouslyCertified = prior.Certified,
            PreviousBillCount = prior.Bills.Count,
            PendingDraftBillNumber = prior.PendingDraftBillNumber,
            Lines = order.Details.OrderBy(d => d.Id).Select(d => new EwoBillableLineDto
            {
                EngineerWorkOrderDetailId = d.Id,
                MaterialName = d.Material?.Name ?? string.Empty,
                UnitOfMeasure = d.UnitOfMeasure,
                OrderedQuantity = d.Qty,
                Rate = d.Rate,
                PreviousMeasuredQuantity = PreviousQty(d)
            }).ToList(),
            Heads = order.PaymentHeads.OrderBy(h => h.SortOrder).Select(h =>
            {
                prior.ClaimedByHead.TryGetValue(EngineerWorkOrderPaymentHeads.Key(h.HeadName), out var claimed);
                return new EwoBillableHeadDto
                {
                    EngineerWorkOrderPaymentHeadId = h.Id,
                    HeadName = h.HeadName,
                    Percent = h.Percent,
                    ClaimedPercent = claimed
                };
            }).ToList()
        };
    }
}

public record GetEwoStatementQuery(long EngineerWorkOrderId) : IRequest<EwoStatementDto?>;

public class GetEwoStatementQueryHandler : IRequestHandler<GetEwoStatementQuery, EwoStatementDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetEwoStatementQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<EwoStatementDto?> Handle(GetEwoStatementQuery request, CancellationToken cancellationToken)
    {
        var workOrders = _unitOfWork.Repository<EngineerWorkOrder>().Query();
        var rootId = await workOrders
            .Where(o => o.Id == request.EngineerWorkOrderId)
            .Select(o => (long?)(o.MotherWorkOrderId ?? o.Id))
            .FirstOrDefaultAsync(cancellationToken);
        if (rootId is null)
        {
            return null;
        }

        var latest = await workOrders
            .Include(o => o.Supplier)
            .Include(o => o.EngineerWorkOrderRequisition).ThenInclude(r => r.Project)
            .Include(o => o.PaymentHeads)
            .Where(o => o.Id == rootId || o.MotherWorkOrderId == rootId)
            .OrderByDescending(o => o.RevisionNo)
            .FirstAsync(cancellationToken);

        var bills = await _unitOfWork.Repository<EwoBill>().Query()
            .Include(b => b.EngineerWorkOrder)
            .Include(b => b.Supplier)
            .Include(b => b.Heads)
            .Where(b => b.RootWorkOrderId == rootId && b.IsActive && b.Status != PoBillStatus.Cancelled)
            .OrderBy(b => b.Id)
            .ToListAsync(cancellationToken);

        var billDtos = _mapper.Map<List<EwoBillDto>>(bills);
        await EwoBillPayments.FillPaidAsync(_unitOfWork, billDtos, cancellationToken);

        // Heads of the latest revision, plus any head claimed earlier that a revision later renamed or dropped.
        var claims = bills.SelectMany(b => b.Heads.Select(h => new { b.BillNumber, h.HeadName, h.HeadPercent, h.ClaimPercent })).ToList();
        var heads = latest.PaymentHeads.OrderBy(h => h.SortOrder)
            .Select(h => (h.HeadName, h.Percent))
            .ToList();
        heads.AddRange(claims
            .Where(c => heads.All(h => EngineerWorkOrderPaymentHeads.Key(h.HeadName) != EngineerWorkOrderPaymentHeads.Key(c.HeadName)))
            .GroupBy(c => EngineerWorkOrderPaymentHeads.Key(c.HeadName))
            .Select(g => (g.First().HeadName, g.First().HeadPercent))
            .ToList());

        return new EwoStatementDto
        {
            RootWorkOrderId = rootId.Value,
            WorkOrderNo = latest.WorkOrderNo,
            SupplierName = latest.Supplier?.Name ?? string.Empty,
            ProjectName = latest.EngineerWorkOrderRequisition?.Project?.Name ?? string.Empty,
            ContractAmount = latest.TotalAmount,
            LatestMeasuredAmount = bills.LastOrDefault()?.MeasuredAmount ?? 0,
            Bills = billDtos,
            Heads = heads.Select(h =>
            {
                var headClaims = claims.Where(c => EngineerWorkOrderPaymentHeads.Key(c.HeadName) == EngineerWorkOrderPaymentHeads.Key(h.HeadName)).ToList();
                return new EwoStatementHeadDto
                {
                    HeadName = h.HeadName,
                    Percent = h.Percent,
                    ClaimedPercent = headClaims.Sum(c => c.ClaimPercent),
                    BillNumbers = string.Join(", ", headClaims.Select(c => c.BillNumber).Distinct())
                };
            }).ToList(),
            CompanyName = await _unitOfWork.Repository<Company>().Query()
                .Where(c => c.IsActive).Select(c => c.Name).FirstOrDefaultAsync(cancellationToken)
        };
    }
}
