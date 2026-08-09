using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.EquipmentRentals;

public record CreateEquipmentRentalCommand(CreateEquipmentRentalDto Dto) : IRequest<Guid>;

public class CreateEquipmentRentalCommandHandler : IRequestHandler<CreateEquipmentRentalCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateEquipmentRentalCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateEquipmentRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = _mapper.Map<EquipmentRental>(request.Dto);
        rental.TotalAmount = rental.RentalEndDate.HasValue
            ? (decimal)(rental.RentalEndDate.Value - rental.RentalStartDate).TotalDays * rental.RatePerDay
            : 0m;

        await _unitOfWork.Repository<EquipmentRental>().AddAsync(rental);
        await _unitOfWork.SaveChangesAsync();
        return rental.Id;
    }
}
