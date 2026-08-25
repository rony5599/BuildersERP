using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.GoodsReceives;

public record CreateGoodsReceiveCommand(CreateGoodsReceiveDto Dto) : IRequest<long>;

public class CreateGoodsReceiveCommandHandler : IRequestHandler<CreateGoodsReceiveCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreateGoodsReceiveCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _numberGenerator = numberGenerator;
    }

    public async Task<long> Handle(CreateGoodsReceiveCommand request, CancellationToken cancellationToken)
    {
        var projectId = await _unitOfWork.Repository<PurchaseOrder>().Query()
            .Where(o => o.Id == request.Dto.PurchaseOrderId)
            .Select(o => o.VendorQuotation.Rfq.PurchaseRequisition.ProjectId)
            .SingleAsync(cancellationToken);

        var receive = _mapper.Map<GoodsReceive>(request.Dto);
        receive.GrnNumber = await _numberGenerator.GenerateAsync(projectId, "GRN", cancellationToken);

        decimal receivedAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var amounts = LineItemCalculator.Calculate(detail.ReceivedQuantity, detail.UnitPrice, 0, detail.VatPercent, detail.TaxPercent);
            receive.Details.Add(new GoodsReceiveDetail
            {
                GoodsReceiveId = receive.Id,
                PurchaseOrderDetailId = detail.PurchaseOrderDetailId,
                MaterialId = detail.MaterialId,
                ReceivedQuantity = detail.ReceivedQuantity,
                UnitOfMeasure = detail.UnitOfMeasure,
                UnitPrice = detail.UnitPrice,
                BatchNo = detail.BatchNo,
                SerialNo = detail.SerialNo,
                VatPercent = detail.VatPercent,
                VatAmount = amounts.VatAmount,
                TaxPercent = detail.TaxPercent,
                TaxAmount = amounts.TaxAmount,
                LineTotal = amounts.NetAmount
            });
            receivedAmount += amounts.NetAmount;
        }

        receive.ReceivedAmount = receivedAmount;

        await _unitOfWork.Repository<GoodsReceive>().AddAsync(receive);
        await _unitOfWork.SaveChangesAsync();

        return receive.Id;
    }
}
