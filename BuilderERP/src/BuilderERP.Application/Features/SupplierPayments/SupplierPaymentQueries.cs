using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SupplierPayments;

public record GetAllSupplierPaymentsQuery(
    int Page = 1,
    int PageSize = 25,
    string? Search = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<SupplierPaymentDto>>;

public class GetAllSupplierPaymentsQueryHandler : IRequestHandler<GetAllSupplierPaymentsQuery, PagedResult<SupplierPaymentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSupplierPaymentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<SupplierPaymentDto>> Handle(GetAllSupplierPaymentsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SupplierPayment>().Query()
            .Include(p => p.PoBill)
            .Include(p => p.Supplier)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(p => p.PaymentNumber.Contains(term) || p.PoBill.BillNumber.Contains(term) || p.Supplier.Name.Contains(term));
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(p => p.PaymentDate >= request.DateFrom.Value.Date);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(p => p.PaymentDate < request.DateTo.Value.Date.AddDays(1));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var payments = await query
            .OrderByDescending(p => p.PaymentDate).ThenByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<SupplierPaymentDto>(_mapper.Map<IReadOnlyList<SupplierPaymentDto>>(payments), totalCount, page, pageSize);
    }
}

public record GetPayableBillsQuery : IRequest<IReadOnlyList<PayableBillDto>>;

public class GetPayableBillsQueryHandler : IRequestHandler<GetPayableBillsQuery, IReadOnlyList<PayableBillDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPayableBillsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<PayableBillDto>> Handle(GetPayableBillsQuery request, CancellationToken cancellationToken)
    {
        var bills = await _unitOfWork.Repository<PoBill>().Query()
            .Where(b => b.IsActive && b.Status == PoBillStatus.Approved)
            .OrderByDescending(b => b.BillDate)
            .Select(b => new PayableBillDto
            {
                Id = b.Id,
                BillNumber = b.BillNumber,
                PONumber = b.PurchaseOrder.PONumber,
                SupplierName = b.PurchaseOrder.VendorQuotation.Supplier.Name,
                TotalAmount = b.TotalAmount
            })
            .ToListAsync(cancellationToken);

        var paid = await _unitOfWork.Repository<SupplierPayment>().Query()
            .Where(p => p.IsActive)
            .GroupBy(p => p.PoBillId)
            .Select(g => new { g.Key, Amount = g.Sum(x => x.Amount) })
            .ToDictionaryAsync(x => x.Key, x => x.Amount, cancellationToken);

        foreach (var bill in bills)
        {
            paid.TryGetValue(bill.Id, out var amount);
            bill.PaidAmount = amount;
        }

        return bills.Where(b => b.Outstanding > 0).ToList();
    }
}
