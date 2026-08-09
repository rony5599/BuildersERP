using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SafetyAudits;

public record UpdateSafetyAuditCommand(UpdateSafetyAuditDto Dto) : IRequest<bool>;

public class UpdateSafetyAuditCommandHandler : IRequestHandler<UpdateSafetyAuditCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSafetyAuditCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSafetyAuditCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SafetyAudit>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.ProjectId = request.Dto.ProjectId;
        item.AuditDate = request.Dto.AuditDate;
        item.AuditedBy = request.Dto.AuditedBy;
        item.Score = request.Dto.Score;
        item.Findings = request.Dto.Findings;
        item.Status = request.Dto.Status;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
