using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PerformanceEvaluations;

public record GetAllPerformanceEvaluationsQuery(long? ContractorId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<PerformanceEvaluationDto>>;

public class GetAllPerformanceEvaluationsQueryHandler : IRequestHandler<GetAllPerformanceEvaluationsQuery, PagedResult<PerformanceEvaluationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPerformanceEvaluationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<PerformanceEvaluationDto>> Handle(GetAllPerformanceEvaluationsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PerformanceEvaluation>().Query()
            .Include(x => x.Contractor)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ContractorId.HasValue)
        {
            query = query.Where(x => x.ContractorId == request.ContractorId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var evaluations = await query
            .OrderByDescending(x => x.EvaluationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<PerformanceEvaluationDto>>(evaluations);
        return new PagedResult<PerformanceEvaluationDto>(items, totalCount, page, pageSize);
    }
}
