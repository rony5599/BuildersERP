using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PoBills;

public record GetAllPoBillsQuery(
    int Page = 1,
    int PageSize = 25,
    string? BillNumber = null,
    string? Supplier = null,
    PoBillStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<PoBillDto>>;

public class GetAllPoBillsQueryHandler : IRequestHandler<GetAllPoBillsQuery, PagedResult<PoBillDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPoBillsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<PoBillDto>> Handle(GetAllPoBillsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PoBill>().Query()
            .Include(b => b.PurchaseOrder).ThenInclude(o => o.VendorQuotation).ThenInclude(v => v.Supplier)
            .Include(b => b.PurchaseOrder).ThenInclude(o => o.VendorQuotation).ThenInclude(v => v.Rfq).ThenInclude(r => r.PurchaseRequisition).ThenInclude(pr => pr.Project)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.BillNumber))
        {
            var term = request.BillNumber.Trim();
            query = query.Where(b => b.BillNumber.Contains(term) || b.PurchaseOrder.PONumber.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.Supplier))
        {
            var term = request.Supplier.Trim();
            query = query.Where(b => b.PurchaseOrder.VendorQuotation.Supplier.Name.Contains(term));
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

        var items = _mapper.Map<IReadOnlyList<PoBillDto>>(bills);
        return new PagedResult<PoBillDto>(items, totalCount, page, pageSize);
    }
}
