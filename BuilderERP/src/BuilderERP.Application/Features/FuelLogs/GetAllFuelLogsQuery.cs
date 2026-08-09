using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.FuelLogs;

public record GetAllFuelLogsQuery(Guid? EquipmentId = null) : IRequest<IReadOnlyList<FuelLogDto>>;

public class GetAllFuelLogsQueryHandler : IRequestHandler<GetAllFuelLogsQuery, IReadOnlyList<FuelLogDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllFuelLogsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<FuelLogDto>> Handle(GetAllFuelLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<FuelLog>().Query()
            .Include(x => x.Equipment)
            .AsQueryable();

        if (request.EquipmentId.HasValue)
        {
            query = query.Where(x => x.EquipmentId == request.EquipmentId.Value);
        }

        var logs = await query
            .OrderByDescending(x => x.LogDate)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<FuelLogDto>>(logs);
    }
}
