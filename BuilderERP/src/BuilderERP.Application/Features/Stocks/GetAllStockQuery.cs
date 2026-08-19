using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Stocks;

public record GetAllStockQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<StockDto>>;

public class GetAllStockQueryHandler : IRequestHandler<GetAllStockQuery, PagedResult<StockDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllStockQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<StockDto>> Handle(GetAllStockQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Stock>().Query()
            .Include(s => s.Material)
            .Include(s => s.Warehouse).ThenInclude(w => w.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(s => s.Warehouse.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var stocks = await query
            .OrderBy(s => s.Material.Name)
            .ThenBy(s => s.Warehouse.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<StockDto>>(stocks);
        return new PagedResult<StockDto>(items, totalCount, page, pageSize);
    }
}
