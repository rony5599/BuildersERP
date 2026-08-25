using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseReturns;

public record GetPurchaseReturnByIdQuery(long Id) : IRequest<PurchaseReturnDto?>;

public class GetPurchaseReturnByIdQueryHandler : IRequestHandler<GetPurchaseReturnByIdQuery, PurchaseReturnDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPurchaseReturnByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PurchaseReturnDto?> Handle(GetPurchaseReturnByIdQuery request, CancellationToken cancellationToken)
    {
        var purchaseReturn = await _unitOfWork.Repository<PurchaseReturn>().Query()
            .Include(r => r.GoodsReceive).ThenInclude(g => g.PurchaseOrder).ThenInclude(o => o.VendorQuotation).ThenInclude(v => v.Rfq).ThenInclude(rfq => rfq.PurchaseRequisition).ThenInclude(pr => pr.Project)
            .Include(r => r.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        return purchaseReturn is null ? null : _mapper.Map<PurchaseReturnDto>(purchaseReturn);
    }
}
