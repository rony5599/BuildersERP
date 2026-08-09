using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.MaintenanceRecords;

public record GetAllMaintenanceRecordsQuery(Guid? EquipmentId = null) : IRequest<IReadOnlyList<MaintenanceRecordDto>>;

public class GetAllMaintenanceRecordsQueryHandler : IRequestHandler<GetAllMaintenanceRecordsQuery, IReadOnlyList<MaintenanceRecordDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMaintenanceRecordsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<MaintenanceRecordDto>> Handle(GetAllMaintenanceRecordsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<MaintenanceRecord>().Query()
            .Include(x => x.Equipment)
            .AsQueryable();

        if (request.EquipmentId.HasValue)
        {
            query = query.Where(x => x.EquipmentId == request.EquipmentId.Value);
        }

        var records = await query
            .OrderByDescending(x => x.MaintenanceDate)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<MaintenanceRecordDto>>(records);
    }
}
