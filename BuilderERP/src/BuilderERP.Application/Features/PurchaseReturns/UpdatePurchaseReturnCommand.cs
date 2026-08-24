using BuilderERP.Application.Common;
using BuilderERP.Application.Common.Caching;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseReturns;

public record UpdatePurchaseReturnCommand(UpdatePurchaseReturnDto Dto) : IRequest<UpdatePurchaseReturnResult>, IInvalidatesFeatures
{
    // Approving/completing a return posts stock deductions via PurchaseReturnPostingService,
    // so Stocks' cached queries must be invalidated too, not just PurchaseReturns' own.
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["Stocks"];
}

public enum UpdatePurchaseReturnResult
{
    Success,
    NotFound,
    Locked,
    OverReturn
}

public class UpdatePurchaseReturnCommandHandler : IRequestHandler<UpdatePurchaseReturnCommand, UpdatePurchaseReturnResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePurchaseReturnCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdatePurchaseReturnResult> Handle(UpdatePurchaseReturnCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PurchaseReturn>();
        var purchaseReturn = await repository.Query()
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == request.Dto.Id, cancellationToken);

        if (purchaseReturn is null)
        {
            return UpdatePurchaseReturnResult.NotFound;
        }

        if (purchaseReturn.Status is PurchaseReturnStatus.Approved or PurchaseReturnStatus.Completed)
        {
            return UpdatePurchaseReturnResult.Locked;
        }

        var isApproving = request.Dto.Status is PurchaseReturnStatus.Approved or PurchaseReturnStatus.Completed;

        purchaseReturn.ReturnNumber = request.Dto.ReturnNumber;
        purchaseReturn.ReturnDate = request.Dto.ReturnDate;
        purchaseReturn.Reason = request.Dto.Reason;
        purchaseReturn.GoodsReceiveId = request.Dto.GoodsReceiveId;

        var detailRepository = _unitOfWork.Repository<PurchaseReturnDetail>();
        foreach (var detail in purchaseReturn.Details.ToList())
        {
            detailRepository.Remove(detail);
        }

        purchaseReturn.Details.Clear();

        var newDetails = new List<PurchaseReturnDetail>();
        decimal returnAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var amounts = LineItemCalculator.Calculate(detail.ReturnQuantity, detail.UnitPrice, 0, detail.VatPercent, detail.TaxPercent);
            var newDetail = new PurchaseReturnDetail
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
            };
            await detailRepository.AddAsync(newDetail);
            newDetails.Add(newDetail);
            returnAmount += amounts.NetAmount;
        }

        purchaseReturn.ReturnAmount = returnAmount;

        if (isApproving)
        {
            var isValid = await PurchaseReturnPostingService.ValidateAsync(_unitOfWork, purchaseReturn, newDetails, cancellationToken);
            if (!isValid)
            {
                return UpdatePurchaseReturnResult.OverReturn;
            }
        }

        purchaseReturn.Status = request.Dto.Status;
        repository.Update(purchaseReturn);

        if (isApproving)
        {
            await PurchaseReturnPostingService.ApplyAsync(_unitOfWork, purchaseReturn, newDetails, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync();
        return UpdatePurchaseReturnResult.Success;
    }
}
