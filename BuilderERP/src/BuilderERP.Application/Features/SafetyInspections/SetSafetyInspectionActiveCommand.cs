using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SafetyInspections;

public record SetSafetyInspectionActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetSafetyInspectionActiveCommandHandler : IRequestHandler<SetSafetyInspectionActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSafetyInspectionActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSafetyInspectionActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SafetyInspection>();
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
