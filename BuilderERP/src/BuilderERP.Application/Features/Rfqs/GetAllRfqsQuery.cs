using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Rfqs;

public record GetAllRfqsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<RfqDto>>;

public class GetAllRfqsQueryHandler : IRequestHandler<GetAllRfqsQuery, PagedResult<RfqDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllRfqsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<RfqDto>> Handle(GetAllRfqsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Rfq>().Query()
            .Include(r => r.PurchaseRequisition).ThenInclude(pr => pr.Project)
            .Include(r => r.RfqVendors).ThenInclude(v => v.Supplier)
            .Include(r => r.Details)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var rfqs = await query
            .OrderByDescending(r => r.IssueDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<RfqDto>>(rfqs);
        return new PagedResult<RfqDto>(items, totalCount, page, pageSize);
    }
}
