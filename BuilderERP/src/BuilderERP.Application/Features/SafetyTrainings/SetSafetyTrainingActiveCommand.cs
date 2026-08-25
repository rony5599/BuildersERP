using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SafetyTrainings;

public record SetSafetyTrainingActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetSafetyTrainingActiveCommandHandler : IRequestHandler<SetSafetyTrainingActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSafetyTrainingActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSafetyTrainingActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SafetyTraining>();
        var training = await repository.GetByIdAsync(request.Id);
        if (training is null)
        {
            return false;
        }

        training.IsActive = request.IsActive;
        repository.Update(training);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
