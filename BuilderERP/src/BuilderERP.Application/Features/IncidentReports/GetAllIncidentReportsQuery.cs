using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.IncidentReports;

public record GetAllIncidentReportsQuery(long? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<IncidentReportDto>>;

public class GetAllIncidentReportsQueryHandler : IRequestHandler<GetAllIncidentReportsQuery, PagedResult<IncidentReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllIncidentReportsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<IncidentReportDto>> Handle(GetAllIncidentReportsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<IncidentReport>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var reports = await query
            .OrderByDescending(x => x.IncidentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<IncidentReportDto>>(reports);
        return new PagedResult<IncidentReportDto>(items, totalCount, page, pageSize);
    }
}
