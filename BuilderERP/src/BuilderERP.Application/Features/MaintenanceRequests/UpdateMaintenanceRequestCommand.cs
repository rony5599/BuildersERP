using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.MaintenanceRequests;

public record UpdateMaintenanceRequestCommand(UpdateMaintenanceRequestDto Dto) : IRequest<bool>;

public class UpdateMaintenanceRequestCommandHandler : IRequestHandler<UpdateMaintenanceRequestCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMaintenanceRequestCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateMaintenanceRequestCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<MaintenanceRequest>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.RequestNumber = request.Dto.RequestNumber;
        item.RequestType = request.Dto.RequestType;
        item.Description = request.Dto.Description;
        item.Priority = request.Dto.Priority;
        item.Status = request.Dto.Status;
        item.RequestDate = request.Dto.RequestDate;
        item.ResolvedDate = request.Dto.ResolvedDate;
        item.AssignedTo = request.Dto.AssignedTo;
        item.Remarks = request.Dto.Remarks;
        item.PropertyUnitId = request.Dto.PropertyUnitId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
