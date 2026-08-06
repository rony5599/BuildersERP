using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Towers;

public record CreateTowerCommand(CreateTowerDto Dto) : IRequest<Guid>;

public class CreateTowerCommandHandler : IRequestHandler<CreateTowerCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTowerCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateTowerCommand request, CancellationToken cancellationToken)
    {
        var tower = _mapper.Map<Tower>(request.Dto);
        await _unitOfWork.Repository<Tower>().AddAsync(tower);
        await _unitOfWork.SaveChangesAsync();

        return tower.Id;
    }
}
