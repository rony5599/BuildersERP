using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.MaintenanceRecords;

public record GetMaintenanceRecordByIdQuery(Guid Id) : IRequest<MaintenanceRecordDto?>;

public class GetMaintenanceRecordByIdQueryHandler : IRequestHandler<GetMaintenanceRecordByIdQuery, MaintenanceRecordDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMaintenanceRecordByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MaintenanceRecordDto?> Handle(GetMaintenanceRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var record = await _unitOfWork.Repository<MaintenanceRecord>().GetByIdAsync(request.Id);
        return record is null ? null : _mapper.Map<MaintenanceRecordDto>(record);
    }
}
