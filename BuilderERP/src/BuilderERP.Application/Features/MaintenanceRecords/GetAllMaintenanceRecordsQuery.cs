using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.MaintenanceRecords;

public record GetAllMaintenanceRecordsQuery(Guid? EquipmentId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<MaintenanceRecordDto>>;

public class GetAllMaintenanceRecordsQueryHandler : IRequestHandler<GetAllMaintenanceRecordsQuery, PagedResult<MaintenanceRecordDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMaintenanceRecordsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<MaintenanceRecordDto>> Handle(GetAllMaintenanceRecordsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<MaintenanceRecord>().Query()
            .Include(x => x.Equipment)
            .AsQueryable();

        if (request.EquipmentId.HasValue)
        {
            query = query.Where(x => x.EquipmentId == request.EquipmentId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var records = await query
            .OrderByDescending(x => x.MaintenanceDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<MaintenanceRecordDto>>(records);
        return new PagedResult<MaintenanceRecordDto>(items, totalCount, page, pageSize);
    }
}
