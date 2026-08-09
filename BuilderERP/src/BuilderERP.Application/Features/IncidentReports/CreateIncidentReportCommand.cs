using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.IncidentReports;

public record CreateIncidentReportCommand(CreateIncidentReportDto Dto) : IRequest<Guid>;

public class CreateIncidentReportCommandHandler : IRequestHandler<CreateIncidentReportCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateIncidentReportCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateIncidentReportCommand request, CancellationToken cancellationToken)
    {
        var report = _mapper.Map<IncidentReport>(request.Dto);
        await _unitOfWork.Repository<IncidentReport>().AddAsync(report);
        await _unitOfWork.SaveChangesAsync();
        return report.Id;
    }
}
