using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.FuelLogs;

public record GetAllFuelLogsQuery(long? EquipmentId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<FuelLogDto>>;

public class GetAllFuelLogsQueryHandler : IRequestHandler<GetAllFuelLogsQuery, PagedResult<FuelLogDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllFuelLogsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<FuelLogDto>> Handle(GetAllFuelLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<FuelLog>().Query()
            .Include(x => x.Equipment)
            .AsQueryable();

        if (request.EquipmentId.HasValue)
        {
            query = query.Where(x => x.EquipmentId == request.EquipmentId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        query = query.OrderByDescending(x => x.LogDate);

        var totalCount = await query.CountAsync(cancellationToken);
        var logs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<FuelLogDto>>(logs);
        return new PagedResult<FuelLogDto>(items, totalCount, page, pageSize);
    }
}
