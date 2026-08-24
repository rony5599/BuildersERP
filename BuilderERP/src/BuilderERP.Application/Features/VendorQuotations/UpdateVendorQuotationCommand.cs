using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VendorQuotations;

public record UpdateVendorQuotationCommand(UpdateVendorQuotationDto Dto) : IRequest<UpdateVendorQuotationResult>;

public enum UpdateVendorQuotationResult
{
    Success,
    NotFound,
    Locked
}

public class UpdateVendorQuotationCommandHandler : IRequestHandler<UpdateVendorQuotationCommand, UpdateVendorQuotationResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVendorQuotationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateVendorQuotationResult> Handle(UpdateVendorQuotationCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<VendorQuotation>();
        var quotation = await repository.Query()
            .Include(v => v.Details)
            .FirstOrDefaultAsync(v => v.Id == request.Dto.Id, cancellationToken);

        if (quotation is null)
        {
            return UpdateVendorQuotationResult.NotFound;
        }

        if (quotation.Status is VendorQuotationStatus.Selected or VendorQuotationStatus.Rejected)
        {
            return UpdateVendorQuotationResult.Locked;
        }

        quotation.QuotationNumber = request.Dto.QuotationNumber;
        quotation.QuotationDate = request.Dto.QuotationDate;
        quotation.DeliveryDays = request.Dto.DeliveryDays;
        quotation.Status = request.Dto.Status;
        quotation.RfqId = request.Dto.RfqId;
        quotation.SupplierId = request.Dto.SupplierId;

        var detailRepository = _unitOfWork.Repository<VendorQuotationDetail>();
        foreach (var detail in quotation.Details.ToList())
        {
            detailRepository.Remove(detail);
        }

        decimal quotedAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var amounts = LineItemCalculator.Calculate(detail.Quantity, detail.UnitPrice, detail.DiscountPercent, detail.VatPercent, detail.TaxPercent);
            await detailRepository.AddAsync(new VendorQuotationDetail
            {
                VendorQuotationId = quotation.Id,
                MaterialId = detail.MaterialId,
                Quantity = detail.Quantity,
                UnitOfMeasure = detail.UnitOfMeasure,
                UnitPrice = detail.UnitPrice,
                DiscountPercent = detail.DiscountPercent,
                DiscountAmount = amounts.DiscountAmount,
                VatPercent = detail.VatPercent,
                VatAmount = amounts.VatAmount,
                TaxPercent = detail.TaxPercent,
                TaxAmount = amounts.TaxAmount,
                NetAmount = amounts.NetAmount,
                DeliveryDays = detail.DeliveryDays
            });
            quotedAmount += amounts.NetAmount;
        }

        quotation.QuotedAmount = quotedAmount;

        repository.Update(quotation);
        await _unitOfWork.SaveChangesAsync();
        return UpdateVendorQuotationResult.Success;
    }
}
