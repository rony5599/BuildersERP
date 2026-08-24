using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.GoodsReceives;

public record CreateGoodsReceiveCommand(CreateGoodsReceiveDto Dto) : IRequest<Guid>;

public class CreateGoodsReceiveCommandHandler : IRequestHandler<CreateGoodsReceiveCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateGoodsReceiveCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateGoodsReceiveCommand request, CancellationToken cancellationToken)
    {
        var receive = _mapper.Map<GoodsReceive>(request.Dto);

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
