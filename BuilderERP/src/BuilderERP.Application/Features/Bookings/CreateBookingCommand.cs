using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Bookings;

public record CreateBookingCommand(CreateBookingDto Dto) : IRequest<long>;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateBookingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = _mapper.Map<Booking>(request.Dto);
        if (booking.Status != BookingRequestStatus.Cancelled)
        {
            booking.CancellationReason = null;
        }

        await _unitOfWork.Repository<Booking>().AddAsync(booking);

        await BookingUnitStatusSync.ApplyAsync(_unitOfWork, booking.PropertyUnitId, booking.Status);

        await _unitOfWork.SaveChangesAsync();

        return booking.Id;
    }
}

internal static class BookingUnitStatusSync
{
    public static async Task ApplyAsync(IUnitOfWork unitOfWork, long propertyUnitId, BookingRequestStatus status)
    {
        BookingStatus? newStatus = status switch
        {
            BookingRequestStatus.Confirmed => BookingStatus.Reserved,
            BookingRequestStatus.Cancelled => BookingStatus.Available,
            _ => null
        };

        if (newStatus is null)
        {
            return;
        }

        var repository = unitOfWork.Repository<PropertyUnit>();
        var unit = await repository.GetByIdAsync(propertyUnitId);
        if (unit is null)
        {
            return;
        }

        unit.BookingStatus = newStatus.Value;
        repository.Update(unit);
    }
}
