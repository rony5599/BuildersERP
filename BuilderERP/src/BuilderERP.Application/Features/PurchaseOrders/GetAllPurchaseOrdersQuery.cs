using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseOrders;

public record GetAllPurchaseOrdersQuery(
    int Page = 1,
    int PageSize = 25,
    string? PONumber = null,
    DateTime? OrderDateFrom = null,
    DateTime? OrderDateTo = null,
    DateTime? DeliveryDateFrom = null,
    DateTime? DeliveryDateTo = null) : IRequest<PagedResult<PurchaseOrderDto>>;

public class GetAllPurchaseOrdersQueryHandler : IRequestHandler<GetAllPurchaseOrdersQuery, PagedResult<PurchaseOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPurchaseOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<PurchaseOrderDto>> Handle(GetAllPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PurchaseOrder>().Query()
            .Include(o => o.VendorQuotation)
            .ThenInclude(v => v.Supplier)
            .Include(o => o.VendorQuotation)
            .ThenInclude(v => v.Rfq)
            .ThenInclude(r => r.PurchaseRequisition)
            .ThenInclude(p => p.Project)
            .Include(o => o.Details)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.PONumber))
        {
            query = query.Where(o => o.PONumber.Contains(request.PONumber));
        }

        if (request.OrderDateFrom.HasValue)
        {
            var from = request.OrderDateFrom.Value.Date;
            query = query.Where(o => o.OrderDate >= from);
        }

        if (request.OrderDateTo.HasValue)
        {
            var to = request.OrderDateTo.Value.Date.AddDays(1);
            query = query.Where(o => o.OrderDate < to);
        }

        if (request.DeliveryDateFrom.HasValue)
        {
            var from = request.DeliveryDateFrom.Value.Date;
            query = query.Where(o => o.DeliveryDate >= from);
        }

        if (request.DeliveryDateTo.HasValue)
        {
            var to = request.DeliveryDateTo.Value.Date.AddDays(1);
            query = query.Where(o => o.DeliveryDate < to);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<PurchaseOrderDto>>(orders);
        return new PagedResult<PurchaseOrderDto>(items, totalCount, page, pageSize);
    }
}
