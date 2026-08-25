using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.IncidentReports;

public record SetIncidentReportActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetIncidentReportActiveCommandHandler : IRequestHandler<SetIncidentReportActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetIncidentReportActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetIncidentReportActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<IncidentReport>();
        var report = await repository.GetByIdAsync(request.Id);
        if (report is null)
        {
            return false;
        }

        report.IsActive = request.IsActive;
        repository.Update(report);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
