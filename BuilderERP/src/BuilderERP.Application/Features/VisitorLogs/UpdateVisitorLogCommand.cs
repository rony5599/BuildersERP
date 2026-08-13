using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.VisitorLogs;

public record UpdateVisitorLogCommand(UpdateVisitorLogDto Dto) : IRequest<bool>;

public class UpdateVisitorLogCommandHandler : IRequestHandler<UpdateVisitorLogCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVisitorLogCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateVisitorLogCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<VisitorLog>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.VisitorName = request.Dto.VisitorName;
        item.Phone = request.Dto.Phone;
        item.PurposeOfVisit = request.Dto.PurposeOfVisit;
        item.HostName = request.Dto.HostName;
        item.CheckInTime = request.Dto.CheckInTime;
        item.CheckOutTime = request.Dto.CheckOutTime;
        item.IdProofNumber = request.Dto.IdProofNumber;
        item.VehicleNumber = request.Dto.VehicleNumber;
        item.Remarks = request.Dto.Remarks;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
