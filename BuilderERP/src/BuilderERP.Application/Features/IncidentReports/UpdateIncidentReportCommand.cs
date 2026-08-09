using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.IncidentReports;

public record UpdateIncidentReportCommand(UpdateIncidentReportDto Dto) : IRequest<bool>;

public class UpdateIncidentReportCommandHandler : IRequestHandler<UpdateIncidentReportCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateIncidentReportCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateIncidentReportCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<IncidentReport>();
        var report = await repository.GetByIdAsync(request.Dto.Id);
        if (report is null)
        {
            return false;
        }

        report.ProjectId = request.Dto.ProjectId;
        report.IncidentDate = request.Dto.IncidentDate;
        report.ReportedBy = request.Dto.ReportedBy;
        report.Location = request.Dto.Location;
        report.Description = request.Dto.Description;
        report.Severity = request.Dto.Severity;
        report.InjuredPersonName = request.Dto.InjuredPersonName;
        report.Status = request.Dto.Status;
        report.CorrectiveAction = request.Dto.CorrectiveAction;
        report.ClosedDate = request.Dto.ClosedDate;

        repository.Update(report);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
