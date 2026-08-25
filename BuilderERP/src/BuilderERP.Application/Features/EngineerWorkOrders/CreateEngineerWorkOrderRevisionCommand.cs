using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EngineerWorkOrders;

public record CreateEngineerWorkOrderRevisionCommand(long PreviousWorkOrderId, CreateEngineerWorkOrderDto Dto) : IRequest<CreateEngineerWorkOrderRevisionResult>;

public enum CreateEngineerWorkOrderRevisionResult
{
    Success,
    NotFound,
    NotLatestRevision
}

public class CreateEngineerWorkOrderRevisionCommandHandler : IRequestHandler<CreateEngineerWorkOrderRevisionCommand, CreateEngineerWorkOrderRevisionResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateEngineerWorkOrderRevisionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateEngineerWorkOrderRevisionResult> Handle(CreateEngineerWorkOrderRevisionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<EngineerWorkOrder>();
        var previous = await repository.Query()
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.Id == request.PreviousWorkOrderId, cancellationToken);

        if (previous is null)
        {
            return CreateEngineerWorkOrderRevisionResult.NotFound;
        }

        if (!previous.IsLatestRevision)
        {
            return CreateEngineerWorkOrderRevisionResult.NotLatestRevision;
        }

        string motherBaseNo;
        if (previous.MotherWorkOrderId is null)
        {
            motherBaseNo = previous.WorkOrderNo;
        }
        else
        {
            var mother = await repository.GetByIdAsync(previous.MotherWorkOrderId.Value);
            motherBaseNo = mother?.WorkOrderNo ?? previous.WorkOrderNo;
        }

        var newRevisionNo = previous.RevisionNo + 1;

        var revision = new EngineerWorkOrder
        {
            WorkOrderNo = $"{motherBaseNo}-R{newRevisionNo:D2}",
            EngineerWorkOrderRequisitionId = request.Dto.EngineerWorkOrderRequisitionId,
            MotherWorkOrderId = previous.MotherWorkOrderId ?? previous.Id,
            PreviousWorkOrderId = previous.Id,
            RevisionNo = newRevisionNo,
            IsLatestRevision = true,
            RevisionDate = DateTime.UtcNow,
            TermsAndCondition = request.Dto.TermsAndCondition,
            SupplierId = request.Dto.SupplierId,
            Status = request.Dto.Status
        };

        decimal totalAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var lineAmount = detail.Qty * detail.Rate;
            revision.Details.Add(new EngineerWorkOrderDetail
            {
                EngineerWorkOrderId = revision.Id,
                MaterialId = detail.MaterialId,
                UnitOfMeasure = detail.UnitOfMeasure,
                Qty = detail.Qty,
                Rate = detail.Rate,
                Amount = lineAmount,
                Remarks = detail.Remarks
            });
            totalAmount += lineAmount;
        }

        revision.TotalAmount = totalAmount;

        previous.IsLatestRevision = false;
        previous.Status = EngineerWorkOrderStatus.Superseded;

        await repository.AddAsync(revision);
        repository.Update(previous);
        await _unitOfWork.SaveChangesAsync();

        return CreateEngineerWorkOrderRevisionResult.Success;
    }
}
