using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EquipmentRentals;

public record GetAllEquipmentRentalsQuery(long? EquipmentId = null, long? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<EquipmentRentalDto>>;

public class GetAllEquipmentRentalsQueryHandler : IRequestHandler<GetAllEquipmentRentalsQuery, PagedResult<EquipmentRentalDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllEquipmentRentalsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<EquipmentRentalDto>> Handle(GetAllEquipmentRentalsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<EquipmentRental>().Query()
            .Include(x => x.Equipment)
            .Include(x => x.Supplier)
            .Include(x => x.Project)
            .AsQueryable();

        if (request.EquipmentId.HasValue)
        {
            query = query.Where(x => x.EquipmentId == request.EquipmentId.Value);
        }

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var rentals = await query
            .OrderByDescending(x => x.RentalStartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<EquipmentRentalDto>>(rentals);
        return new PagedResult<EquipmentRentalDto>(items, totalCount, page, pageSize);
    }
}
