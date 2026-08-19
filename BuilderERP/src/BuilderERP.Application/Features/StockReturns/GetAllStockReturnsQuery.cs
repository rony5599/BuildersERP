using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.StockReturns;

public record GetAllStockReturnsQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<StockReturnDto>>;

public class GetAllStockReturnsQueryHandler : IRequestHandler<GetAllStockReturnsQuery, PagedResult<StockReturnDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllStockReturnsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<StockReturnDto>> Handle(GetAllStockReturnsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<StockReturn>().Query()
            .Include(r => r.Material)
            .Include(r => r.Warehouse).ThenInclude(w => w.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(r => r.Warehouse.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var returns = await query
            .OrderByDescending(r => r.ReturnDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<StockReturnDto>>(returns);
        return new PagedResult<StockReturnDto>(items, totalCount, page, pageSize);
    }
}
