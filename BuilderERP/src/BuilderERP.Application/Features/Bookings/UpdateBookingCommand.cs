using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Bookings;

public record UpdateBookingCommand(UpdateBookingDto Dto) : IRequest<bool>;

public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBookingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Booking>();
        var booking = await repository.GetByIdAsync(request.Dto.Id);
        if (booking is null)
        {
            return false;
        }

        booking.BookingDate = request.Dto.BookingDate;
        booking.BookingAmount = request.Dto.BookingAmount;
        booking.Status = request.Dto.Status;
        booking.CancellationReason = request.Dto.Status == BuilderERP.Domain.Enums.BookingRequestStatus.Cancelled ? request.Dto.CancellationReason : null;
        booking.CustomerId = request.Dto.CustomerId;
        booking.PropertyUnitId = request.Dto.PropertyUnitId;
        booking.BrokerId = request.Dto.BrokerId;

        repository.Update(booking);

        await BookingUnitStatusSync.ApplyAsync(_unitOfWork, booking.PropertyUnitId, booking.Status);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
