using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Overtimes;

public record GetAllOvertimesQuery(Guid? WorkerId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<OvertimeDto>>;

public class GetAllOvertimesQueryHandler : IRequestHandler<GetAllOvertimesQuery, PagedResult<OvertimeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllOvertimesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<OvertimeDto>> Handle(GetAllOvertimesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Overtime>().Query()
            .Include(x => x.Worker)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.WorkerId.HasValue)
        {
            query = query.Where(x => x.WorkerId == request.WorkerId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var overtimes = await query
            .OrderByDescending(x => x.OvertimeDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<OvertimeDto>>(overtimes);
        return new PagedResult<OvertimeDto>(items, totalCount, page, pageSize);
    }
}
