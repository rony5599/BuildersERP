using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VisitorLogs;

public record GetAllVisitorLogsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<VisitorLogDto>>;

public class GetAllVisitorLogsQueryHandler : IRequestHandler<GetAllVisitorLogsQuery, PagedResult<VisitorLogDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllVisitorLogsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<VisitorLogDto>> Handle(GetAllVisitorLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<VisitorLog>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderBy(x => x.VisitorName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<VisitorLogDto>>(results);
        return new PagedResult<VisitorLogDto>(items, totalCount, page, pageSize);
    }
}
