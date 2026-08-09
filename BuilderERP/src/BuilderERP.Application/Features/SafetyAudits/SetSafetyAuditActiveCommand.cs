using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SafetyAudits;

public record SetSafetyAuditActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetSafetyAuditActiveCommandHandler : IRequestHandler<SetSafetyAuditActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSafetyAuditActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSafetyAuditActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SafetyAudit>();
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
