using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DelayEvents;

public record UpdateDelayEventCommand(UpdateDelayEventDto Dto) : IRequest<bool>;

public class UpdateDelayEventCommandHandler : IRequestHandler<UpdateDelayEventCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDelayEventCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDelayEventCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<DelayEvent>();
        var delayEvent = await repository.GetByIdAsync(request.Dto.Id);
        if (delayEvent is null)
        {
            return false;
        }

        delayEvent.Description = request.Dto.Description;
        delayEvent.DelayDays = request.Dto.DelayDays;
        delayEvent.Reason = request.Dto.Reason;
        delayEvent.ReportedDate = request.Dto.ReportedDate;
        delayEvent.WbsTaskId = request.Dto.WbsTaskId;
        delayEvent.ProjectId = request.Dto.ProjectId;

        repository.Update(delayEvent);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
