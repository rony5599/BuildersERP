using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CashPurchaseOrders;

public record GetCashPurchaseOrderByIdQuery(long Id) : IRequest<CashPurchaseOrderDto?>;

public class GetCashPurchaseOrderByIdQueryHandler : IRequestHandler<GetCashPurchaseOrderByIdQuery, CashPurchaseOrderDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCashPurchaseOrderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CashPurchaseOrderDto?> Handle(GetCashPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<CashPurchaseOrder>().Query()
            .Include(o => o.Supplier)
            .Include(o => o.CashRequisition)
            .Include(o => o.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        return order is null ? null : _mapper.Map<CashPurchaseOrderDto>(order);
    }
}
