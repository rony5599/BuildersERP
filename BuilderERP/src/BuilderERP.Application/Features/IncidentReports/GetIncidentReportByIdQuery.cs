using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.IncidentReports;

public record GetIncidentReportByIdQuery(Guid Id) : IRequest<IncidentReportDto?>;

public class GetIncidentReportByIdQueryHandler : IRequestHandler<GetIncidentReportByIdQuery, IncidentReportDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetIncidentReportByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IncidentReportDto?> Handle(GetIncidentReportByIdQuery request, CancellationToken cancellationToken)
    {
        var report = await _unitOfWork.Repository<IncidentReport>().GetByIdAsync(request.Id);
        return report is null ? null : _mapper.Map<IncidentReportDto>(report);
    }
}
