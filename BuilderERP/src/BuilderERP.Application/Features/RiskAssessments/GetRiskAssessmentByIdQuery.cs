using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RiskAssessments;

public record GetRiskAssessmentByIdQuery(long Id) : IRequest<RiskAssessmentDto?>;

public class GetRiskAssessmentByIdQueryHandler : IRequestHandler<GetRiskAssessmentByIdQuery, RiskAssessmentDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRiskAssessmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RiskAssessmentDto?> Handle(GetRiskAssessmentByIdQuery request, CancellationToken cancellationToken)
    {
        var assessment = await _unitOfWork.Repository<RiskAssessment>().GetByIdAsync(request.Id);
        return assessment is null ? null : _mapper.Map<RiskAssessmentDto>(assessment);
    }
}
