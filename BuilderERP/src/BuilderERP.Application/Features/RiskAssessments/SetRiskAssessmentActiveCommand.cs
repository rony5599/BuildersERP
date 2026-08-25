using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RiskAssessments;

public record SetRiskAssessmentActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetRiskAssessmentActiveCommandHandler : IRequestHandler<SetRiskAssessmentActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetRiskAssessmentActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetRiskAssessmentActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<RiskAssessment>();
        var assessment = await repository.GetByIdAsync(request.Id);
        if (assessment is null)
        {
            return false;
        }

        assessment.IsActive = request.IsActive;
        repository.Update(assessment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
