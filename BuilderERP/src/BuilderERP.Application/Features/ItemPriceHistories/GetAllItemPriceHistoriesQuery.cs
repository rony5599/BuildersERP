using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ItemPriceHistories;

public record GetAllItemPriceHistoriesQuery(
    long? MaterialId = null,
    long? SupplierId = null,
    int Page = 1,
    int PageSize = 25,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<ItemPriceHistoryDto>>;

public class GetAllItemPriceHistoriesQueryHandler : IRequestHandler<GetAllItemPriceHistoriesQuery, PagedResult<ItemPriceHistoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllItemPriceHistoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ItemPriceHistoryDto>> Handle(GetAllItemPriceHistoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<ItemPriceHistory>().Query()
            .Include(h => h.Material)
            .Include(h => h.Supplier)
            .AsQueryable();

        if (request.MaterialId.HasValue)
        {
            query = query.Where(h => h.MaterialId == request.MaterialId.Value);
        }

        if (request.SupplierId.HasValue)
        {
            query = query.Where(h => h.SupplierId == request.SupplierId.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(h => h.EffectiveDate >= request.DateFrom.Value.Date);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(h => h.EffectiveDate < request.DateTo.Value.Date.AddDays(1));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var histories = await query
            .OrderByDescending(h => h.EffectiveDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<ItemPriceHistoryDto>>(histories);
        return new PagedResult<ItemPriceHistoryDto>(items, totalCount, page, pageSize);
    }
}
