using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RiskAssessments;

public record CreateRiskAssessmentCommand(CreateRiskAssessmentDto Dto) : IRequest<Guid>;

public class CreateRiskAssessmentCommandHandler : IRequestHandler<CreateRiskAssessmentCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateRiskAssessmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateRiskAssessmentCommand request, CancellationToken cancellationToken)
    {
        var assessment = _mapper.Map<RiskAssessment>(request.Dto);
        await _unitOfWork.Repository<RiskAssessment>().AddAsync(assessment);
        await _unitOfWork.SaveChangesAsync();
        return assessment.Id;
    }
}
