using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PerformanceEvaluations;

public record UpdatePerformanceEvaluationCommand(UpdatePerformanceEvaluationDto Dto) : IRequest<bool>;

public class UpdatePerformanceEvaluationCommandHandler : IRequestHandler<UpdatePerformanceEvaluationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePerformanceEvaluationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdatePerformanceEvaluationCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PerformanceEvaluation>();
        var evaluation = await repository.GetByIdAsync(request.Dto.Id);
        if (evaluation is null)
        {
            return false;
        }

        evaluation.ContractorId = request.Dto.ContractorId;
        evaluation.ProjectId = request.Dto.ProjectId;
        evaluation.EvaluationDate = request.Dto.EvaluationDate;
        evaluation.QualityScore = request.Dto.QualityScore;
        evaluation.TimelinessScore = request.Dto.TimelinessScore;
        evaluation.SafetyScore = request.Dto.SafetyScore;
        evaluation.Remarks = request.Dto.Remarks;

        repository.Update(evaluation);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
