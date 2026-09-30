using BuilderERP.Application.Common.Caching;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EngineerWorkOrders;

public record UpdateEngineerWorkOrderCommand(UpdateEngineerWorkOrderDto Dto) : IRequest<UpdateEngineerWorkOrderResult>, IInvalidatesFeatures
{
    // Payment heads and revisions feed the EWO bill form and statement (cached under EwoBills).
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["EwoBills"];
}

public enum UpdateEngineerWorkOrderResult
{
    Success,
    NotFound,
    Locked
}

public class UpdateEngineerWorkOrderCommandHandler : IRequestHandler<UpdateEngineerWorkOrderCommand, UpdateEngineerWorkOrderResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEngineerWorkOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateEngineerWorkOrderResult> Handle(UpdateEngineerWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<EngineerWorkOrder>();
        var workOrder = await repository.Query()
            .Include(o => o.Details)
            .Include(o => o.PaymentHeads)
            .FirstOrDefaultAsync(o => o.Id == request.Dto.Id, cancellationToken);

        if (workOrder is null)
        {
            return UpdateEngineerWorkOrderResult.NotFound;
        }

        if (workOrder.Status is EngineerWorkOrderStatus.Approved or EngineerWorkOrderStatus.Active
            or EngineerWorkOrderStatus.Superseded or EngineerWorkOrderStatus.Cancelled or EngineerWorkOrderStatus.Rejected)
        {
            return UpdateEngineerWorkOrderResult.Locked;
        }

        workOrder.WorkOrderNo = request.Dto.WorkOrderNo;
        workOrder.EngineerWorkOrderRequisitionId = request.Dto.EngineerWorkOrderRequisitionId;
        workOrder.SupplierId = request.Dto.SupplierId;
        workOrder.TermsAndCondition = request.Dto.TermsAndCondition;
        workOrder.Status = request.Dto.Status;

        var detailRepository = _unitOfWork.Repository<EngineerWorkOrderDetail>();
        foreach (var detail in workOrder.Details.ToList())
        {
            detailRepository.Remove(detail);
        }

        decimal totalAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var lineAmount = detail.Qty * detail.Rate;
            await detailRepository.AddAsync(new EngineerWorkOrderDetail
            {
                EngineerWorkOrderId = workOrder.Id,
                MaterialId = detail.MaterialId,
                UnitOfMeasure = detail.UnitOfMeasure,
                Qty = detail.Qty,
                Rate = detail.Rate,
                Amount = lineAmount,
                Remarks = detail.Remarks
            });
            totalAmount += lineAmount;
        }

        workOrder.TotalAmount = totalAmount;

        var headRepository = _unitOfWork.Repository<EngineerWorkOrderPaymentHead>();
        foreach (var head in workOrder.PaymentHeads.ToList())
        {
            headRepository.Remove(head);
        }

        foreach (var head in EngineerWorkOrderPaymentHeads.ToEntities(request.Dto.PaymentHeads, workOrder.Id))
        {
            await headRepository.AddAsync(head);
        }

        repository.Update(workOrder);
        await _unitOfWork.SaveChangesAsync();
        return UpdateEngineerWorkOrderResult.Success;
    }
}
