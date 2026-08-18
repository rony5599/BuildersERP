using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Towers;

public record GetAllTowersQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<TowerDto>>;

public class GetAllTowersQueryHandler : IRequestHandler<GetAllTowersQuery, IReadOnlyList<TowerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllTowersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TowerDto>> Handle(GetAllTowersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Tower>().Query()
            .Include(t => t.Building).ThenInclude(b => b.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(t => t.Building.ProjectId == request.ProjectId.Value);
        }

        var towers = await query.ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<TowerDto>>(towers);
    }
}
