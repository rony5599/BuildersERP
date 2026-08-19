using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Leads;

public record GetAllLeadsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<LeadDto>>;

public class GetAllLeadsQueryHandler : IRequestHandler<GetAllLeadsQuery, PagedResult<LeadDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllLeadsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<LeadDto>> Handle(GetAllLeadsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Lead>().Query().Include(l => l.AssignedToUser).AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var leads = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<LeadDto>>(leads);
        return new PagedResult<LeadDto>(items, totalCount, page, pageSize);
    }
}
