using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseRequisitions;

public record GetAllPurchaseRequisitionsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<PurchaseRequisitionDto>>;

public class GetAllPurchaseRequisitionsQueryHandler : IRequestHandler<GetAllPurchaseRequisitionsQuery, PagedResult<PurchaseRequisitionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPurchaseRequisitionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<PurchaseRequisitionDto>> Handle(GetAllPurchaseRequisitionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PurchaseRequisition>().Query()
            .Include(r => r.Department)
            .Include(r => r.Details)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var requisitions = await query
            .OrderByDescending(r => r.RequestDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<PurchaseRequisitionDto>>(requisitions);
        return new PagedResult<PurchaseRequisitionDto>(items, totalCount, page, pageSize);
    }
}
