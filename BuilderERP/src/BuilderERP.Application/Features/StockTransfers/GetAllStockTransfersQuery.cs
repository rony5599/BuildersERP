using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.StockTransfers;

public record GetAllStockTransfersQuery(
    long? ProjectId = null,
    int Page = 1,
    int PageSize = 25,
    string? TransferNumber = null,
    long? MaterialId = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResult<StockTransferDto>>;

public class GetAllStockTransfersQueryHandler : IRequestHandler<GetAllStockTransfersQuery, PagedResult<StockTransferDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllStockTransfersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<StockTransferDto>> Handle(GetAllStockTransfersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<StockTransfer>().Query()
            .Include(t => t.Material)
            .Include(t => t.FromWarehouse).ThenInclude(w => w.Project)
            .Include(t => t.ToWarehouse).ThenInclude(w => w.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(t => t.FromWarehouse.ProjectId == request.ProjectId.Value || t.ToWarehouse.ProjectId == request.ProjectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TransferNumber))
        {
            var term = request.TransferNumber.Trim();
            query = query.Where(t => t.TransferNumber.Contains(term));
        }

        if (request.MaterialId.HasValue)
        {
            query = query.Where(t => t.MaterialId == request.MaterialId.Value);
        }

        if (request.DateFrom.HasValue)
        {
            query = query.Where(t => t.TransferDate >= request.DateFrom.Value.Date);
        }

        if (request.DateTo.HasValue)
        {
            query = query.Where(t => t.TransferDate < request.DateTo.Value.Date.AddDays(1));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var transfers = await query
            .OrderByDescending(t => t.TransferDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<StockTransferDto>>(transfers);
        return new PagedResult<StockTransferDto>(items, totalCount, page, pageSize);
    }
}
