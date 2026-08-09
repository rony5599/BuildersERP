using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.QualityChecklists;

public record SetQualityChecklistActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetQualityChecklistActiveCommandHandler : IRequestHandler<SetQualityChecklistActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetQualityChecklistActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetQualityChecklistActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<QualityChecklist>();
        var checklist = await repository.GetByIdAsync(request.Id);
        if (checklist is null)
        {
            return false;
        }

        checklist.IsActive = request.IsActive;
        repository.Update(checklist);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
