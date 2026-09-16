using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CashPurchaseOrders;

public record GetAllCashPurchaseOrdersQuery(
    int Page = 1,
    int PageSize = 25,
    string? CpoNumber = null,
    long? ProjectId = null,
    PurchaseOrderStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<CashPurchaseOrderDto>>;

public class GetAllCashPurchaseOrdersQueryHandler : IRequestHandler<GetAllCashPurchaseOrdersQuery, PagedResult<CashPurchaseOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCashPurchaseOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<CashPurchaseOrderDto>> Handle(GetAllCashPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<CashPurchaseOrder>().Query()
            .Include(o => o.Supplier)
            .Include(o => o.CashRequisition)
            .ThenInclude(r => r.Project)
            .Include(o => o.Details)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.CpoNumber))
        {
            var term = request.CpoNumber.Trim();
            query = query.Where(o => o.CPONumber.Contains(term));
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(o => o.CashRequisition.ProjectId == request.ProjectId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(o => o.Status == request.Status.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(o => o.OrderDate >= request.DateFrom.Value.Date);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(o => o.OrderDate < request.DateTo.Value.Date.AddDays(1));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<CashPurchaseOrderDto>>(orders);
        return new PagedResult<CashPurchaseOrderDto>(items, totalCount, page, pageSize);
    }
}
