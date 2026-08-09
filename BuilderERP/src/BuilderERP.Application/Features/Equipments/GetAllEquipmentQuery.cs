using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Equipments;

public record GetAllEquipmentQuery : IRequest<IReadOnlyList<EquipmentDto>>;

public class GetAllEquipmentQueryHandler : IRequestHandler<GetAllEquipmentQuery, IReadOnlyList<EquipmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllEquipmentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<EquipmentDto>> Handle(GetAllEquipmentQuery request, CancellationToken cancellationToken)
    {
        var equipment = await _unitOfWork.Repository<Equipment>().Query()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<EquipmentDto>>(equipment);
    }
}
