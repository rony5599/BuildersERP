using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VendorQuotations;

public record GetAllVendorQuotationsQuery(
    int Page = 1,
    int PageSize = 25,
    string? QuotationNumber = null,
    long? ProjectId = null,
    VendorQuotationStatus? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<VendorQuotationDto>>;

public class GetAllVendorQuotationsQueryHandler : IRequestHandler<GetAllVendorQuotationsQuery, PagedResult<VendorQuotationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllVendorQuotationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<VendorQuotationDto>> Handle(GetAllVendorQuotationsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<VendorQuotation>().Query()
            .Include(v => v.Supplier)
            .Include(v => v.Rfq)
            .ThenInclude(r => r.PurchaseRequisition)
            .ThenInclude(pr => pr.Project)
            .Include(v => v.Details)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.QuotationNumber))
        {
            var term = request.QuotationNumber.Trim();
            query = query.Where(v => v.QuotationNumber.Contains(term));
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(v => v.Rfq.PurchaseRequisition.ProjectId == request.ProjectId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(v => v.Status == request.Status.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(v => v.QuotationDate >= request.DateFrom.Value.Date);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(v => v.QuotationDate < request.DateTo.Value.Date.AddDays(1));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var quotations = await query
            .OrderByDescending(v => v.QuotationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<VendorQuotationDto>>(quotations);
        return new PagedResult<VendorQuotationDto>(items, totalCount, page, pageSize);
    }
}
