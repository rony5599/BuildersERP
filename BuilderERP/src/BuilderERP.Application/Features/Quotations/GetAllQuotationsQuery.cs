using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Quotations;

public record GetAllQuotationsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<QuotationDto>>;

public class GetAllQuotationsQueryHandler : IRequestHandler<GetAllQuotationsQuery, PagedResult<QuotationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllQuotationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<QuotationDto>> Handle(GetAllQuotationsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Quotation>().Query()
            .Include(q => q.Customer)
            .Include(q => q.PropertyUnit);

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var quotations = await query
            .OrderByDescending(q => q.ValidUntil)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<QuotationDto>>(quotations);
        return new PagedResult<QuotationDto>(items, totalCount, page, pageSize);
    }
}
