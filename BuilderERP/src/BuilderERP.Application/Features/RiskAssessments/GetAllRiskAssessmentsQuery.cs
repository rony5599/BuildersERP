using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.RiskAssessments;

public record GetAllRiskAssessmentsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<RiskAssessmentDto>>;

public class GetAllRiskAssessmentsQueryHandler : IRequestHandler<GetAllRiskAssessmentsQuery, IReadOnlyList<RiskAssessmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllRiskAssessmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<RiskAssessmentDto>> Handle(GetAllRiskAssessmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<RiskAssessment>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var assessments = await query
            .OrderByDescending(x => x.AssessmentDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<RiskAssessmentDto>>(assessments);
    }
}
