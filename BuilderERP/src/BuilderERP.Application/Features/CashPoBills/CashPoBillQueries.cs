using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CashPoBills;

public record GetAllCashPoBillsQuery(
    int Page = 1,
    int PageSize = 25,
    string? BillNumber = null,
    string? Requester = null,
    PoBillStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<CashPoBillDto>>;

public class GetAllCashPoBillsQueryHandler : IRequestHandler<GetAllCashPoBillsQuery, PagedResult<CashPoBillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCashPoBillsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<CashPoBillDto>> Handle(GetAllCashPoBillsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<CashPoBill>().Query()
            .Include(b => b.CashPurchaseOrder).ThenInclude(o => o.Supplier)
            .Include(b => b.CashPurchaseOrder).ThenInclude(o => o.CashRequisition)
            .Include(b => b.RequesterEmployee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.BillNumber))
        {
            var term = request.BillNumber.Trim();
            query = query.Where(b => b.BillNumber.Contains(term) || b.CashPurchaseOrder.CPONumber.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.Requester))
        {
            var term = request.Requester.Trim();
            query = query.Where(b => b.RequesterEmployee.EmployeeName.Contains(term));
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

        return new PagedResult<CashPoBillDto>(_mapper.Map<IReadOnlyList<CashPoBillDto>>(bills), totalCount, page, pageSize);
    }
}

public record GetCashPoBillByIdQuery(long Id) : IRequest<CashPoBillDto?>;

public class GetCashPoBillByIdQueryHandler : IRequestHandler<GetCashPoBillByIdQuery, CashPoBillDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCashPoBillByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CashPoBillDto?> Handle(GetCashPoBillByIdQuery request, CancellationToken cancellationToken)
    {
        var bill = await _unitOfWork.Repository<CashPoBill>().Query()
            .Include(b => b.CashPurchaseOrder).ThenInclude(o => o.Supplier)
            .Include(b => b.CashPurchaseOrder).ThenInclude(o => o.CashRequisition)
            .Include(b => b.RequesterEmployee)
            .Include(b => b.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        return bill is null ? null : _mapper.Map<CashPoBillDto>(bill);
    }
}

public record GetCashPoBillPrintDataQuery(long Id) : IRequest<CashPoBillPrintDto?>;

public class GetCashPoBillPrintDataQueryHandler : IRequestHandler<GetCashPoBillPrintDataQuery, CashPoBillPrintDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCashPoBillPrintDataQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CashPoBillPrintDto?> Handle(GetCashPoBillPrintDataQuery request, CancellationToken cancellationToken)
    {
        var bill = await _unitOfWork.Repository<CashPoBill>().Query()
            .Include(b => b.CashPurchaseOrder).ThenInclude(o => o.Supplier)
            .Include(b => b.CashPurchaseOrder).ThenInclude(o => o.CashRequisition).ThenInclude(r => r.Project)
            .Include(b => b.RequesterEmployee)
            .Include(b => b.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (bill is null)
        {
            return null;
        }

        var company = await _unitOfWork.Repository<Company>().Query()
            .Where(c => c.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        var order = bill.CashPurchaseOrder;
        var supplier = order.Supplier;

        return new CashPoBillPrintDto
        {
            BillNumber = bill.BillNumber,
            BillDate = bill.BillDate,
            MemoNumber = bill.MemoNumber,
            CPONumber = order.CPONumber,
            CPODate = order.OrderDate,
            RequisitionNumber = order.CashRequisition?.RequisitionNumber ?? string.Empty,
            RequesterName = bill.RequesterEmployee?.EmployeeName ?? string.Empty,
            RequesterCode = bill.RequesterEmployee?.EmployeeCode,
            RequesterMobile = bill.RequesterEmployee?.MobileNo,
            ProjectName = order.CashRequisition?.Project?.Name,
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
            Lines = bill.Details.Select(d => new PoBillPrintLineDto
            {
                Description = d.Material?.Name ?? string.Empty,
                Quantity = d.BilledQuantity,
                Rate = d.UnitPrice,
                Discount = d.DiscountAmount,
                Vat = d.VatAmount,
                Tax = d.TaxAmount,
                Amount = d.LineTotal
            }).ToList()
        };
    }
}

public record GetBillableCashPurchaseOrdersQuery(long? IncludeOrderId = null) : IRequest<IReadOnlyList<BillableCashPurchaseOrderDto>>;

public class GetBillableCashPurchaseOrdersQueryHandler : IRequestHandler<GetBillableCashPurchaseOrdersQuery, IReadOnlyList<BillableCashPurchaseOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBillableCashPurchaseOrdersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<BillableCashPurchaseOrderDto>> Handle(GetBillableCashPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var statuses = CashPoBillLineBuilder.BillableStatuses;
        return await _unitOfWork.Repository<CashPurchaseOrder>().Query()
            .Where(o => (o.IsActive && statuses.Contains(o.Status)) || o.Id == request.IncludeOrderId)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new BillableCashPurchaseOrderDto
            {
                Id = o.Id,
                CPONumber = o.CPONumber,
                RequisitionNumber = o.CashRequisition.RequisitionNumber,
                RequesterName = o.CashRequisition.RequesterEmployee.EmployeeName,
                SupplierName = o.Supplier.Name
            })
            .ToListAsync(cancellationToken);
    }
}

public record GetCashPoBillableLinesQuery(long CashPurchaseOrderId, long? ExcludeBillId = null) : IRequest<IReadOnlyList<CashPoBillableLineDto>>;

public class GetCashPoBillableLinesQueryHandler : IRequestHandler<GetCashPoBillableLinesQuery, IReadOnlyList<CashPoBillableLineDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCashPoBillableLinesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<CashPoBillableLineDto>> Handle(GetCashPoBillableLinesQuery request, CancellationToken cancellationToken)
    {
        var billed = await CashPoBillLineBuilder.BilledQuantitiesAsync(_unitOfWork, request.CashPurchaseOrderId, request.ExcludeBillId, cancellationToken);
        var lines = await _unitOfWork.Repository<CashPurchaseOrderDetail>().Query()
            .Where(d => d.CashPurchaseOrderId == request.CashPurchaseOrderId)
            .Select(d => new CashPoBillableLineDto
            {
                CashPurchaseOrderDetailId = d.Id,
                MaterialId = d.MaterialId,
                MaterialName = d.Material.Name,
                UnitOfMeasure = d.UnitOfMeasure,
                OrderedQuantity = d.OrderedQuantity,
                UnitPrice = d.UnitPrice,
                DiscountPercent = d.DiscountPercent,
                VatPercent = d.VatPercent,
                TaxPercent = d.TaxPercent
            })
            .ToListAsync(cancellationToken);

        foreach (var line in lines)
        {
            billed.TryGetValue(line.CashPurchaseOrderDetailId, out var qty);
            line.AlreadyBilledQuantity = qty;
        }

        return lines;
    }
}
