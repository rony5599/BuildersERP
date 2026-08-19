using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.StockAdjustments;

public record GetAllStockAdjustmentsQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<StockAdjustmentDto>>;

public class GetAllStockAdjustmentsQueryHandler : IRequestHandler<GetAllStockAdjustmentsQuery, PagedResult<StockAdjustmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllStockAdjustmentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<StockAdjustmentDto>> Handle(GetAllStockAdjustmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<StockAdjustment>().Query()
            .Include(a => a.Material)
            .Include(a => a.Warehouse).ThenInclude(w => w.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(a => a.Warehouse.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var adjustments = await query
            .OrderByDescending(a => a.AdjustmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<StockAdjustmentDto>>(adjustments);
        return new PagedResult<StockAdjustmentDto>(items, totalCount, page, pageSize);
    }
}
