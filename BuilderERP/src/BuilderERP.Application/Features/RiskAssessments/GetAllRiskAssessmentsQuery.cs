using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.RiskAssessments;

public record GetAllRiskAssessmentsQuery(long? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<RiskAssessmentDto>>;

public class GetAllRiskAssessmentsQueryHandler : IRequestHandler<GetAllRiskAssessmentsQuery, PagedResult<RiskAssessmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllRiskAssessmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<RiskAssessmentDto>> Handle(GetAllRiskAssessmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<RiskAssessment>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var assessments = await query
            .OrderByDescending(x => x.AssessmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<RiskAssessmentDto>>(assessments);
        return new PagedResult<RiskAssessmentDto>(items, totalCount, page, pageSize);
    }
}
