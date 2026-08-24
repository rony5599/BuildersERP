using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseReturns;

public record CreatePurchaseReturnCommand(CreatePurchaseReturnDto Dto) : IRequest<Guid>;

public class CreatePurchaseReturnCommandHandler : IRequestHandler<CreatePurchaseReturnCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreatePurchaseReturnCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _numberGenerator = numberGenerator;
    }

    public async Task<Guid> Handle(CreatePurchaseReturnCommand request, CancellationToken cancellationToken)
    {
        var projectId = await _unitOfWork.Repository<GoodsReceive>().Query()
            .Where(g => g.Id == request.Dto.GoodsReceiveId)
            .Select(g => g.PurchaseOrder.VendorQuotation.Rfq.PurchaseRequisition.ProjectId)
            .SingleAsync(cancellationToken);

        var purchaseReturn = _mapper.Map<PurchaseReturn>(request.Dto);
        purchaseReturn.ReturnNumber = await _numberGenerator.GenerateAsync(projectId, "PRTN", cancellationToken);

        decimal returnAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var amounts = LineItemCalculator.Calculate(detail.ReturnQuantity, detail.UnitPrice, 0, detail.VatPercent, detail.TaxPercent);
            purchaseReturn.Details.Add(new PurchaseReturnDetail
            {
                PurchaseReturnId = purchaseReturn.Id,
                GoodsReceiveDetailId = detail.GoodsReceiveDetailId,
                MaterialId = detail.MaterialId,
                ReturnQuantity = detail.ReturnQuantity,
                UnitOfMeasure = detail.UnitOfMeasure,
                UnitPrice = detail.UnitPrice,
                VatPercent = detail.VatPercent,
                VatAmount = amounts.VatAmount,
                TaxPercent = detail.TaxPercent,
                TaxAmount = amounts.TaxAmount,
                LineTotal = amounts.NetAmount
            });
            returnAmount += amounts.NetAmount;
        }

        purchaseReturn.ReturnAmount = returnAmount;

        await _unitOfWork.Repository<PurchaseReturn>().AddAsync(purchaseReturn);
        await _unitOfWork.SaveChangesAsync();
        return purchaseReturn.Id;
    }
}
