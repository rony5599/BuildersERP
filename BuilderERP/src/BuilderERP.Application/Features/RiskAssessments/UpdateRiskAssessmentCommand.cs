using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RiskAssessments;

public record UpdateRiskAssessmentCommand(UpdateRiskAssessmentDto Dto) : IRequest<bool>;

public class UpdateRiskAssessmentCommandHandler : IRequestHandler<UpdateRiskAssessmentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRiskAssessmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateRiskAssessmentCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<RiskAssessment>();
        var assessment = await repository.GetByIdAsync(request.Dto.Id);
        if (assessment is null)
        {
            return false;
        }

        assessment.ProjectId = request.Dto.ProjectId;
        assessment.AssessmentDate = request.Dto.AssessmentDate;
        assessment.AssessedBy = request.Dto.AssessedBy;
        assessment.HazardDescription = request.Dto.HazardDescription;
        assessment.RiskLevel = request.Dto.RiskLevel;
        assessment.MitigationMeasures = request.Dto.MitigationMeasures;
        assessment.ReviewDate = request.Dto.ReviewDate;

        repository.Update(assessment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
