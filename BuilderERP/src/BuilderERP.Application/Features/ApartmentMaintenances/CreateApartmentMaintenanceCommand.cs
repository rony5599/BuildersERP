using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ApartmentMaintenances;

public record CreateApartmentMaintenanceCommand(CreateApartmentMaintenanceDto Dto) : IRequest<long>;

public class CreateApartmentMaintenanceCommandHandler : IRequestHandler<CreateApartmentMaintenanceCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateApartmentMaintenanceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateApartmentMaintenanceCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<ApartmentMaintenance>(request.Dto);
        var repository = _unitOfWork.Repository<ApartmentMaintenance>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
