using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseReturns;

public record GetAllPurchaseReturnsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<PurchaseReturnDto>>;

public class GetAllPurchaseReturnsQueryHandler : IRequestHandler<GetAllPurchaseReturnsQuery, PagedResult<PurchaseReturnDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPurchaseReturnsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<PurchaseReturnDto>> Handle(GetAllPurchaseReturnsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PurchaseReturn>().Query()
            .Include(r => r.GoodsReceive)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var returns = await query
            .OrderByDescending(r => r.ReturnDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<PurchaseReturnDto>>(returns);
        return new PagedResult<PurchaseReturnDto>(items, totalCount, page, pageSize);
    }
}
