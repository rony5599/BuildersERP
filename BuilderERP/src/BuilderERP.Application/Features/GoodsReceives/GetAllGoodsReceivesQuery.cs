using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.GoodsReceives;

public record GetAllGoodsReceivesQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<GoodsReceiveDto>>;

public class GetAllGoodsReceivesQueryHandler : IRequestHandler<GetAllGoodsReceivesQuery, PagedResult<GoodsReceiveDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllGoodsReceivesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<GoodsReceiveDto>> Handle(GetAllGoodsReceivesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<GoodsReceive>().Query()
            .Include(g => g.PurchaseOrder)
            .Include(g => g.Warehouse)
            .Include(g => g.Details)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var receives = await query
            .OrderByDescending(g => g.ReceivedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<GoodsReceiveDto>>(receives);
        return new PagedResult<GoodsReceiveDto>(items, totalCount, page, pageSize);
    }
}
