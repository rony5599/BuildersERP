using BuilderERP.Application.Common.Caching;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EngineerWorkOrders;

public record SetEngineerWorkOrderPaymentHeadsCommand(SaveEngineerWorkOrderPaymentHeadsDto Dto) : IRequest<SetEngineerWorkOrderPaymentHeadsResult>, IInvalidatesFeatures
{
    // Payment heads and revisions feed the EWO bill form and statement (cached under EwoBills).
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["EwoBills"];
}

public enum SetEngineerWorkOrderPaymentHeadsResult
{
    Success,
    NotFound,
    NotLatestRevision,
    AlreadyBilled
}

// Lets payment heads be set on an approved work order (whose form is otherwise locked),
// but only until the first bill is raised against any of its revisions.
public class SetEngineerWorkOrderPaymentHeadsCommandHandler : IRequestHandler<SetEngineerWorkOrderPaymentHeadsCommand, SetEngineerWorkOrderPaymentHeadsResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetEngineerWorkOrderPaymentHeadsCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SetEngineerWorkOrderPaymentHeadsResult> Handle(SetEngineerWorkOrderPaymentHeadsCommand request, CancellationToken cancellationToken)
    {
        var workOrder = await _unitOfWork.Repository<EngineerWorkOrder>().Query()
            .Include(o => o.PaymentHeads)
            .FirstOrDefaultAsync(o => o.Id == request.Dto.EngineerWorkOrderId, cancellationToken);

        if (workOrder is null)
        {
            return SetEngineerWorkOrderPaymentHeadsResult.NotFound;
        }

        if (!workOrder.IsLatestRevision)
        {
            return SetEngineerWorkOrderPaymentHeadsResult.NotLatestRevision;
        }

        var rootId = workOrder.MotherWorkOrderId ?? workOrder.Id;
        var billed = await _unitOfWork.Repository<EwoBill>().Query()
            .AnyAsync(b => b.RootWorkOrderId == rootId && b.IsActive && b.Status != PoBillStatus.Cancelled, cancellationToken);
        if (billed)
        {
            return SetEngineerWorkOrderPaymentHeadsResult.AlreadyBilled;
        }

        var headRepository = _unitOfWork.Repository<EngineerWorkOrderPaymentHead>();
        foreach (var head in workOrder.PaymentHeads.ToList())
        {
            headRepository.Remove(head);
        }

        foreach (var head in EngineerWorkOrderPaymentHeads.ToEntities(request.Dto.PaymentHeads, workOrder.Id))
        {
            await headRepository.AddAsync(head);
        }

        await _unitOfWork.SaveChangesAsync();
        return SetEngineerWorkOrderPaymentHeadsResult.Success;
    }
}
