using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.GoodsReceives;

public record GetAllGoodsReceivesQuery(
    int Page = 1,
    int PageSize = 25,
    string? GrnNumber = null,
    long? ProjectId = null,
    GrnStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<GoodsReceiveDto>>;

public class GetAllGoodsReceivesQueryHandler : IRequestHandler<GetAllGoodsReceivesQuery, PagedResult<GoodsReceiveDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllGoodsReceivesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<GoodsReceiveDto>> Handle(GetAllGoodsReceivesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<GoodsReceive>().Query()
            .Include(g => g.PurchaseOrder).ThenInclude(o => o!.VendorQuotation).ThenInclude(v => v.Rfq).ThenInclude(r => r.PurchaseRequisition).ThenInclude(pr => pr.Project)
            .Include(g => g.EngineerWorkOrder).ThenInclude(o => o!.EngineerWorkOrderRequisition).ThenInclude(r => r.Project)
            .Include(g => g.CashPurchaseOrder).ThenInclude(o => o!.CashRequisition).ThenInclude(r => r.Project)
            .Include(g => g.Warehouse)
            .Include(g => g.Details)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.GrnNumber))
        {
            var term = request.GrnNumber.Trim();
            query = query.Where(g => g.GrnNumber.Contains(term));
        }

        if (request.ProjectId.HasValue)
        {
            var projectId = request.ProjectId.Value;
            query = query.Where(g =>
                (g.SourceType == GrnSourceType.PurchaseOrder && g.PurchaseOrder != null && g.PurchaseOrder.VendorQuotation != null && g.PurchaseOrder.VendorQuotation.Rfq != null && g.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition != null && g.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.ProjectId == projectId) ||
                (g.SourceType == GrnSourceType.EngineerWorkOrder && g.EngineerWorkOrder != null && g.EngineerWorkOrder.EngineerWorkOrderRequisition != null && g.EngineerWorkOrder.EngineerWorkOrderRequisition.ProjectId == projectId) ||
                (g.SourceType == GrnSourceType.CashPurchaseOrder && g.CashPurchaseOrder != null && g.CashPurchaseOrder.CashRequisition != null && g.CashPurchaseOrder.CashRequisition.ProjectId == projectId));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(g => g.Status == request.Status.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(g => g.ReceivedDate >= request.DateFrom.Value.Date);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(g => g.ReceivedDate < request.DateTo.Value.Date.AddDays(1));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var receives = await query
            .OrderByDescending(g => g.ReceivedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<GoodsReceiveDto>>(receives);
        return new PagedResult<GoodsReceiveDto>(items, totalCount, page, pageSize);
    }
}
