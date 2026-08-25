using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Equipments;

public record CreateEquipmentCommand(CreateEquipmentDto Dto) : IRequest<long>;

public class CreateEquipmentCommandHandler : IRequestHandler<CreateEquipmentCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateEquipmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = _mapper.Map<Equipment>(request.Dto);
        await _unitOfWork.Repository<Equipment>().AddAsync(equipment);
        await _unitOfWork.SaveChangesAsync();
        return equipment.Id;
    }
}
