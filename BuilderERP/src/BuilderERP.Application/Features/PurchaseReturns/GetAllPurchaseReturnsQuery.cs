using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseReturns;

public record GetAllPurchaseReturnsQuery(
    int Page = 1,
    int PageSize = 25,
    string? ReturnNumber = null,
    long? ProjectId = null,
    PurchaseReturnStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<PurchaseReturnDto>>;

public class GetAllPurchaseReturnsQueryHandler : IRequestHandler<GetAllPurchaseReturnsQuery, PagedResult<PurchaseReturnDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPurchaseReturnsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<PurchaseReturnDto>> Handle(GetAllPurchaseReturnsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PurchaseReturn>().Query()
            .Include(r => r.GoodsReceive).ThenInclude(g => g.PurchaseOrder).ThenInclude(o => o.VendorQuotation).ThenInclude(v => v.Rfq).ThenInclude(rfq => rfq.PurchaseRequisition).ThenInclude(pr => pr.Project)
            .Include(r => r.Details)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.ReturnNumber))
        {
            var term = request.ReturnNumber.Trim();
            query = query.Where(r => r.ReturnNumber.Contains(term));
        }

        if (request.ProjectId.HasValue)
        {
            var projectId = request.ProjectId.Value;
            query = query.Where(r =>
                r.GoodsReceive.PurchaseOrder != null &&
                r.GoodsReceive.PurchaseOrder.VendorQuotation != null &&
                r.GoodsReceive.PurchaseOrder.VendorQuotation.Rfq != null &&
                r.GoodsReceive.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition != null &&
                r.GoodsReceive.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.ProjectId == projectId);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(r => r.ReturnDate >= request.DateFrom.Value.Date);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(r => r.ReturnDate < request.DateTo.Value.Date.AddDays(1));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var returns = await query
            .OrderByDescending(r => r.ReturnDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<PurchaseReturnDto>>(returns);
        return new PagedResult<PurchaseReturnDto>(items, totalCount, page, pageSize);
    }
}
