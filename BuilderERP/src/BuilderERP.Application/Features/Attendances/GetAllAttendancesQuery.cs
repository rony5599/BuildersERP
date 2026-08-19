using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Attendances;

public record GetAllAttendancesQuery(Guid? WorkerId = null, Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<AttendanceDto>>;

public class GetAllAttendancesQueryHandler : IRequestHandler<GetAllAttendancesQuery, PagedResult<AttendanceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllAttendancesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<AttendanceDto>> Handle(GetAllAttendancesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Attendance>().Query()
            .Include(x => x.Worker)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.WorkerId.HasValue)
        {
            query = query.Where(x => x.WorkerId == request.WorkerId.Value);
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var attendances = await query
            .OrderByDescending(x => x.AttendanceDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<AttendanceDto>>(attendances);
        return new PagedResult<AttendanceDto>(items, totalCount, page, pageSize);
    }
}
