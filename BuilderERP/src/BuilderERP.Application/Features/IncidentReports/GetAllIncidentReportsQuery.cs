using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.IncidentReports;

public record GetAllIncidentReportsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<IncidentReportDto>>;

public class GetAllIncidentReportsQueryHandler : IRequestHandler<GetAllIncidentReportsQuery, IReadOnlyList<IncidentReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllIncidentReportsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<IncidentReportDto>> Handle(GetAllIncidentReportsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<IncidentReport>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var reports = await query
            .OrderByDescending(x => x.IncidentDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<IncidentReportDto>>(reports);
    }
}
