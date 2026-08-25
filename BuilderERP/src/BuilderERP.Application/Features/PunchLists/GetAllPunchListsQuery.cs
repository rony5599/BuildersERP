using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PunchLists;

public record GetAllPunchListsQuery(long? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<PunchListDto>>;

public class GetAllPunchListsQueryHandler : IRequestHandler<GetAllPunchListsQuery, PagedResult<PunchListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPunchListsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<PunchListDto>> Handle(GetAllPunchListsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PunchList>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.DueDate == null)
            .ThenBy(x => x.DueDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<IReadOnlyList<PunchListDto>>(items);
        return new PagedResult<PunchListDto>(mapped, totalCount, page, pageSize);
    }
}
