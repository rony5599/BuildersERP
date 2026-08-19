using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ParkingSlots;

public record GetAllParkingSlotsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<ParkingSlotDto>>;

public class GetAllParkingSlotsQueryHandler : IRequestHandler<GetAllParkingSlotsQuery, PagedResult<ParkingSlotDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllParkingSlotsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ParkingSlotDto>> Handle(GetAllParkingSlotsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<ParkingSlot>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.SlotNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<IReadOnlyList<ParkingSlotDto>>(items);
        return new PagedResult<ParkingSlotDto>(mapped, totalCount, page, pageSize);
    }
}
