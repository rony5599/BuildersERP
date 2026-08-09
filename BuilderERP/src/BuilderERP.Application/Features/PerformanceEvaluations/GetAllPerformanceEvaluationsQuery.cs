using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PerformanceEvaluations;

public record GetAllPerformanceEvaluationsQuery(Guid? ContractorId = null) : IRequest<IReadOnlyList<PerformanceEvaluationDto>>;

public class GetAllPerformanceEvaluationsQueryHandler : IRequestHandler<GetAllPerformanceEvaluationsQuery, IReadOnlyList<PerformanceEvaluationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPerformanceEvaluationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PerformanceEvaluationDto>> Handle(GetAllPerformanceEvaluationsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PerformanceEvaluation>().Query()
            .Include(x => x.Contractor)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ContractorId.HasValue)
        {
            query = query.Where(x => x.ContractorId == request.ContractorId.Value);
        }

        var evaluations = await query
            .OrderByDescending(x => x.EvaluationDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<PerformanceEvaluationDto>>(evaluations);
    }
}
