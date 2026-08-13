using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CommonAreaBookings;

public record SetCommonAreaBookingActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetCommonAreaBookingActiveCommandHandler : IRequestHandler<SetCommonAreaBookingActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetCommonAreaBookingActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetCommonAreaBookingActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CommonAreaBooking>();
        var item = await repository.GetByIdAsync(request.Id);
        if (item is null)
        {
            return false;
        }

        item.IsActive = request.IsActive;
        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
