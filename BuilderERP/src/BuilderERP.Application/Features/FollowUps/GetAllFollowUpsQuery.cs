using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.FollowUps;

public record GetAllFollowUpsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<FollowUpDto>>;

public class GetAllFollowUpsQueryHandler : IRequestHandler<GetAllFollowUpsQuery, PagedResult<FollowUpDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllFollowUpsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<FollowUpDto>> Handle(GetAllFollowUpsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<FollowUp>().Query()
            .Include(f => f.Lead)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var followUps = await query
            .OrderByDescending(f => f.FollowUpDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<FollowUpDto>>(followUps);
        return new PagedResult<FollowUpDto>(items, totalCount, page, pageSize);
    }
}
