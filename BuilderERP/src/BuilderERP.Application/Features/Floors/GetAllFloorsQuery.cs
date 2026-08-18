using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Floors;

public record GetAllFloorsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<FloorDto>>;

public class GetAllFloorsQueryHandler : IRequestHandler<GetAllFloorsQuery, IReadOnlyList<FloorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllFloorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<FloorDto>> Handle(GetAllFloorsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Floor>().Query()
            .Include(f => f.Tower).ThenInclude(t => t.Building).ThenInclude(b => b.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(f => f.Tower.Building.ProjectId == request.ProjectId.Value);
        }

        var floors = await query.ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<FloorDto>>(floors);
    }
}
