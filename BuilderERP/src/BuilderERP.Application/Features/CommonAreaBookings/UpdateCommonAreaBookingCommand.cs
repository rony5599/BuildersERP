using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CommonAreaBookings;

public record UpdateCommonAreaBookingCommand(UpdateCommonAreaBookingDto Dto) : IRequest<bool>;

public class UpdateCommonAreaBookingCommandHandler : IRequestHandler<UpdateCommonAreaBookingCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCommonAreaBookingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCommonAreaBookingCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CommonAreaBooking>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.BookingNumber = request.Dto.BookingNumber;
        item.FacilityName = request.Dto.FacilityName;
        item.BookingDate = request.Dto.BookingDate;
        item.StartTime = request.Dto.StartTime;
        item.EndTime = request.Dto.EndTime;
        item.Fee = request.Dto.Fee;
        item.Status = request.Dto.Status;
        item.Remarks = request.Dto.Remarks;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
