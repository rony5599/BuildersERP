using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DailyProgresses;

public record GetAllDailyProgressesQuery(long? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<DailyProgressDto>>;

public class GetAllDailyProgressesQueryHandler : IRequestHandler<GetAllDailyProgressesQuery, PagedResult<DailyProgressDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDailyProgressesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<DailyProgressDto>> Handle(GetAllDailyProgressesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<DailyProgress>().Query().Include(x => x.Project).AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.ProgressDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<DailyProgressDto>>(items);
        return new PagedResult<DailyProgressDto>(dtos, totalCount, page, pageSize);
    }
}
