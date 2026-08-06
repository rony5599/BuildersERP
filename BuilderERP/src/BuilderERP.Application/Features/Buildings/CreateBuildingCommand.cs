using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Buildings;

public record CreateBuildingCommand(CreateBuildingDto Dto) : IRequest<Guid>;

public class CreateBuildingCommandHandler : IRequestHandler<CreateBuildingCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBuildingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateBuildingCommand request, CancellationToken cancellationToken)
    {
        var building = _mapper.Map<Building>(request.Dto);
        await _unitOfWork.Repository<Building>().AddAsync(building);
        await _unitOfWork.SaveChangesAsync();

        return building.Id;
    }
}
