using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Bookings;

public record SetBookingActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetBookingActiveCommandHandler : IRequestHandler<SetBookingActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetBookingActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetBookingActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Booking>();
        var booking = await repository.GetByIdAsync(request.Id);
        if (booking is null)
        {
            return false;
        }

        booking.IsActive = request.IsActive;
        repository.Update(booking);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
