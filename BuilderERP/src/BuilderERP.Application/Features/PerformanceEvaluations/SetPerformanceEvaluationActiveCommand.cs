using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PerformanceEvaluations;

public record SetPerformanceEvaluationActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetPerformanceEvaluationActiveCommandHandler : IRequestHandler<SetPerformanceEvaluationActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetPerformanceEvaluationActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetPerformanceEvaluationActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PerformanceEvaluation>();
        var evaluation = await repository.GetByIdAsync(request.Id);
        if (evaluation is null)
        {
            return false;
        }

        evaluation.IsActive = request.IsActive;
        repository.Update(evaluation);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
