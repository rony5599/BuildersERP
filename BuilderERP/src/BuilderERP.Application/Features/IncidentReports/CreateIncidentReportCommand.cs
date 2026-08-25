using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.IncidentReports;

public record CreateIncidentReportCommand(CreateIncidentReportDto Dto) : IRequest<long>;

public class CreateIncidentReportCommandHandler : IRequestHandler<CreateIncidentReportCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateIncidentReportCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateIncidentReportCommand request, CancellationToken cancellationToken)
    {
        var report = _mapper.Map<IncidentReport>(request.Dto);
        await _unitOfWork.Repository<IncidentReport>().AddAsync(report);
        await _unitOfWork.SaveChangesAsync();
        return report.Id;
    }
}
