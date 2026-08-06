using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SaleAgreements;

public record CreateSaleAgreementCommand(CreateSaleAgreementDto Dto) : IRequest<Guid>;

public class CreateSaleAgreementCommandHandler : IRequestHandler<CreateSaleAgreementCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSaleAgreementCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateSaleAgreementCommand request, CancellationToken cancellationToken)
    {
        var agreement = _mapper.Map<SaleAgreement>(request.Dto);
        await _unitOfWork.Repository<SaleAgreement>().AddAsync(agreement);

        await SaleAgreementUnitStatusSync.ApplyAsync(_unitOfWork, agreement.BookingId, agreement.Status);

        await _unitOfWork.SaveChangesAsync();

        return agreement.Id;
    }
}

internal static class SaleAgreementUnitStatusSync
{
    public static async Task ApplyAsync(IUnitOfWork unitOfWork, Guid bookingId, AgreementStatus status)
    {
        BookingStatus? newStatus = status switch
        {
            AgreementStatus.Signed => BookingStatus.Sold,
            AgreementStatus.Cancelled => BookingStatus.Available,
            _ => null
        };

        if (newStatus is null)
        {
            return;
        }

        var bookingRepository = unitOfWork.Repository<Booking>();
        var booking = await bookingRepository.GetByIdAsync(bookingId);
        if (booking is null)
        {
            return;
        }

        var unitRepository = unitOfWork.Repository<PropertyUnit>();
        var unit = await unitRepository.GetByIdAsync(booking.PropertyUnitId);
        if (unit is null)
        {
            return;
        }

        unit.BookingStatus = newStatus.Value;
        unitRepository.Update(unit);
    }
}
