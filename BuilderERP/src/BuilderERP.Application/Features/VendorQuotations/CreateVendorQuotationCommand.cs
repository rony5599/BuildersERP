using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VendorQuotations;

public record CreateVendorQuotationCommand(CreateVendorQuotationDto Dto) : IRequest<Guid>;

public class CreateVendorQuotationCommandHandler : IRequestHandler<CreateVendorQuotationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreateVendorQuotationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _numberGenerator = numberGenerator;
    }

    public async Task<Guid> Handle(CreateVendorQuotationCommand request, CancellationToken cancellationToken)
    {
        var projectId = await _unitOfWork.Repository<Rfq>().Query()
            .Where(r => r.Id == request.Dto.RfqId)
            .Select(r => r.PurchaseRequisition.ProjectId)
            .SingleAsync(cancellationToken);

        var quotation = _mapper.Map<VendorQuotation>(request.Dto);
        quotation.QuotationNumber = await _numberGenerator.GenerateAsync(projectId, "VQ", cancellationToken);

        decimal quotedAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var amounts = LineItemCalculator.Calculate(detail.Quantity, detail.UnitPrice, detail.DiscountPercent, detail.VatPercent, detail.TaxPercent);
            quotation.Details.Add(new VendorQuotationDetail
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

        await _unitOfWork.Repository<VendorQuotation>().AddAsync(quotation);
        await _unitOfWork.SaveChangesAsync();
        return quotation.Id;
    }
}
