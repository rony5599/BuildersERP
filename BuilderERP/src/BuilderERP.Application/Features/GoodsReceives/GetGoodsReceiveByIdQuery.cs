using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.GoodsReceives;

public record GetGoodsReceiveByIdQuery(long Id) : IRequest<GoodsReceiveDto?>;

public class GetGoodsReceiveByIdQueryHandler : IRequestHandler<GetGoodsReceiveByIdQuery, GoodsReceiveDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetGoodsReceiveByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GoodsReceiveDto?> Handle(GetGoodsReceiveByIdQuery request, CancellationToken cancellationToken)
    {
        var receive = await _unitOfWork.Repository<GoodsReceive>().Query()
            .Include(g => g.PurchaseOrder).ThenInclude(o => o!.VendorQuotation).ThenInclude(v => v.Rfq).ThenInclude(r => r.PurchaseRequisition).ThenInclude(pr => pr.Project)
            .Include(g => g.EngineerWorkOrder).ThenInclude(o => o!.EngineerWorkOrderRequisition).ThenInclude(r => r.Project)
            .Include(g => g.CashPurchaseOrder).ThenInclude(o => o!.CashRequisition).ThenInclude(r => r.Project)
            .Include(g => g.Warehouse)
            .Include(g => g.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);
        return receive is null ? null : _mapper.Map<GoodsReceiveDto>(receive);
    }
}
