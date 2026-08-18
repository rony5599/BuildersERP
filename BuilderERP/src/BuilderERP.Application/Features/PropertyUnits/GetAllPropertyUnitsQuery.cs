using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PropertyUnits;

public record GetAllPropertyUnitsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<PropertyUnitDto>>;

public class GetAllPropertyUnitsQueryHandler : IRequestHandler<GetAllPropertyUnitsQuery, IReadOnlyList<PropertyUnitDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPropertyUnitsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PropertyUnitDto>> Handle(GetAllPropertyUnitsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PropertyUnit>().Query()
            .Include(u => u.Floor).ThenInclude(f => f.Tower).ThenInclude(t => t.Building).ThenInclude(b => b.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(u => u.Floor.Tower.Building.ProjectId == request.ProjectId.Value);
        }

        var units = await query.ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PropertyUnitDto>>(units);
    }
}
