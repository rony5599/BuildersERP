using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Towers;

public record GetTowerByIdQuery(Guid Id) : IRequest<TowerDto?>;

public class GetTowerByIdQueryHandler : IRequestHandler<GetTowerByIdQuery, TowerDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTowerByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TowerDto?> Handle(GetTowerByIdQuery request, CancellationToken cancellationToken)
    {
        var tower = await _unitOfWork.Repository<Tower>().GetByIdAsync(request.Id);
        return tower is null ? null : _mapper.Map<TowerDto>(tower);
    }
}
