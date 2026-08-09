using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Overtimes;

public record GetAllOvertimesQuery(Guid? WorkerId = null) : IRequest<IReadOnlyList<OvertimeDto>>;

public class GetAllOvertimesQueryHandler : IRequestHandler<GetAllOvertimesQuery, IReadOnlyList<OvertimeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllOvertimesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<OvertimeDto>> Handle(GetAllOvertimesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Overtime>().Query()
            .Include(x => x.Worker)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.WorkerId.HasValue)
        {
            query = query.Where(x => x.WorkerId == request.WorkerId.Value);
        }

        var overtimes = await query
            .OrderByDescending(x => x.OvertimeDate)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<OvertimeDto>>(overtimes);
    }
}
