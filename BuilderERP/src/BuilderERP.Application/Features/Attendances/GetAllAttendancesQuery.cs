using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Attendances;

public record GetAllAttendancesQuery(Guid? WorkerId = null, Guid? ProjectId = null) : IRequest<IReadOnlyList<AttendanceDto>>;

public class GetAllAttendancesQueryHandler : IRequestHandler<GetAllAttendancesQuery, IReadOnlyList<AttendanceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllAttendancesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AttendanceDto>> Handle(GetAllAttendancesQuery request, CancellationToken cancellationToken)
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

        var attendances = await query
            .OrderByDescending(x => x.AttendanceDate)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<AttendanceDto>>(attendances);
    }
}
