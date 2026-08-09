using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PpeTrackings;

public record UpdatePpeTrackingCommand(UpdatePpeTrackingDto Dto) : IRequest<bool>;

public class UpdatePpeTrackingCommandHandler : IRequestHandler<UpdatePpeTrackingCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePpeTrackingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdatePpeTrackingCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PpeTracking>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.WorkerId = request.Dto.WorkerId;
        item.PpeType = request.Dto.PpeType;
        item.IssueDate = request.Dto.IssueDate;
        item.ExpiryDate = request.Dto.ExpiryDate;
        item.Status = request.Dto.Status;
        item.Remarks = request.Dto.Remarks;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
