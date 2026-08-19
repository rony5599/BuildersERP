using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VendorQuotations;

public record GetAllVendorQuotationsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<VendorQuotationDto>>;

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
            .Include(v => v.Rfq)
            .ThenInclude(r => r.Supplier)
            .Include(v => v.Rfq)
            .ThenInclude(r => r.PurchaseRequisition)
            .AsQueryable();

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
