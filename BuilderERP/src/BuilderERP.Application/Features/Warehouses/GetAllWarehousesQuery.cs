using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Warehouses;

public record GetAllWarehousesQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<WarehouseDto>>;

public class GetAllWarehousesQueryHandler : IRequestHandler<GetAllWarehousesQuery, IReadOnlyList<WarehouseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllWarehousesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<WarehouseDto>> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Warehouse>().Query()
            .Include(w => w.Branch)
            .Include(w => w.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(w => w.ProjectId == request.ProjectId.Value);
        }

        var warehouses = await query.ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<WarehouseDto>>(warehouses);
    }
}
