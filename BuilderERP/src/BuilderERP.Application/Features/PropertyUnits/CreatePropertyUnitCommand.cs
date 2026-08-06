using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PropertyUnits;

public record CreatePropertyUnitCommand(CreatePropertyUnitDto Dto) : IRequest<Guid>;

public class CreatePropertyUnitCommandHandler : IRequestHandler<CreatePropertyUnitCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePropertyUnitCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreatePropertyUnitCommand request, CancellationToken cancellationToken)
    {
        var unit = _mapper.Map<PropertyUnit>(request.Dto);
        await _unitOfWork.Repository<PropertyUnit>().AddAsync(unit);
        await _unitOfWork.SaveChangesAsync();

        return unit.Id;
    }
}
