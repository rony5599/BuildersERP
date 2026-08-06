using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Floors;

public record CreateFloorCommand(CreateFloorDto Dto) : IRequest<Guid>;

public class CreateFloorCommandHandler : IRequestHandler<CreateFloorCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateFloorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateFloorCommand request, CancellationToken cancellationToken)
    {
        var floor = _mapper.Map<Floor>(request.Dto);
        await _unitOfWork.Repository<Floor>().AddAsync(floor);
        await _unitOfWork.SaveChangesAsync();

        return floor.Id;
    }
}
