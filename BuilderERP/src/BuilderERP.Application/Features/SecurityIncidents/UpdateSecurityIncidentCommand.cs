using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SecurityIncidents;

public record UpdateSecurityIncidentCommand(UpdateSecurityIncidentDto Dto) : IRequest<bool>;

public class UpdateSecurityIncidentCommandHandler : IRequestHandler<UpdateSecurityIncidentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSecurityIncidentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSecurityIncidentCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SecurityIncident>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.IncidentNumber = request.Dto.IncidentNumber;
        item.IncidentType = request.Dto.IncidentType;
        item.Location = request.Dto.Location;
        item.IncidentDateTime = request.Dto.IncidentDateTime;
        item.ReportedBy = request.Dto.ReportedBy;
        item.Severity = request.Dto.Severity;
        item.Status = request.Dto.Status;
        item.ActionTaken = request.Dto.ActionTaken;
        item.Remarks = request.Dto.Remarks;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
