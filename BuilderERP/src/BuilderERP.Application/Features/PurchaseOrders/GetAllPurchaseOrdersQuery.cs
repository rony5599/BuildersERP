using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseOrders;

public record GetAllPurchaseOrdersQuery : IRequest<IReadOnlyList<PurchaseOrderDto>>;

public class GetAllPurchaseOrdersQueryHandler : IRequestHandler<GetAllPurchaseOrdersQuery, IReadOnlyList<PurchaseOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPurchaseOrdersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PurchaseOrderDto>> Handle(GetAllPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _unitOfWork.Repository<PurchaseOrder>().Query()
            .Include(o => o.VendorQuotation)
            .ThenInclude(v => v.Rfq)
            .ThenInclude(r => r.Supplier)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PurchaseOrderDto>>(orders);
    }
}
