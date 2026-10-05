using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.EngineerWorkOrderRequisitions;

public record UpdateEngineerWorkOrderRequisitionCommand(UpdateEngineerWorkOrderRequisitionDto Dto) : IRequest<UpdateEngineerWorkOrderRequisitionResult>;

public enum UpdateEngineerWorkOrderRequisitionResult
{
    Success,
    NotFound,
    Locked
}

public class UpdateEngineerWorkOrderRequisitionCommandHandler : IRequestHandler<UpdateEngineerWorkOrderRequisitionCommand, UpdateEngineerWorkOrderRequisitionResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEngineerWorkOrderRequisitionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateEngineerWorkOrderRequisitionResult> Handle(UpdateEngineerWorkOrderRequisitionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<EngineerWorkOrderRequisition>();
        var requisition = await repository.Query()
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == request.Dto.Id, cancellationToken);

        if (requisition is null)
        {
            return UpdateEngineerWorkOrderRequisitionResult.NotFound;
        }

        if (requisition.Status != RequisitionStatus.Draft)
        {
            return UpdateEngineerWorkOrderRequisitionResult.Locked;
        }

        requisition.RequisitionNumber = request.Dto.RequisitionNumber;
        requisition.RequestDate = request.Dto.RequestDate;
        requisition.RequiredByDate = request.Dto.RequiredByDate;
        requisition.Description = request.Dto.Description;
        requisition.Status = request.Dto.Status;
        requisition.ProjectId = request.Dto.ProjectId;

        var detailRepository = _unitOfWork.Repository<EngineerWorkOrderRequisitionDetail>();
        foreach (var detail in requisition.Details.ToList())
        {
            detailRepository.Remove(detail);
        }

        decimal estimatedAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var lineAmount = detail.Quantity * detail.EstimatedUnitPrice;
            await detailRepository.AddAsync(new EngineerWorkOrderRequisitionDetail
            {
                EngineerWorkOrderRequisitionId = requisition.Id,
                MaterialId = detail.MaterialId,
                Quantity = detail.Quantity,
                UnitOfMeasure = detail.UnitOfMeasure,
                EstimatedUnitPrice = detail.EstimatedUnitPrice,
                EstimatedAmount = lineAmount,
                Remarks = detail.Remarks
            });
            estimatedAmount += lineAmount;
        }

        requisition.EstimatedAmount = estimatedAmount;

        repository.Update(requisition);
        await _unitOfWork.SaveChangesAsync();
        return UpdateEngineerWorkOrderRequisitionResult.Success;
    }
}
