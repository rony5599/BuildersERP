using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseRequisitions;

public record UpdatePurchaseRequisitionCommand(UpdatePurchaseRequisitionDto Dto) : IRequest<UpdatePurchaseRequisitionResult>;

public enum UpdatePurchaseRequisitionResult
{
    Success,
    NotFound,
    Locked
}

public class UpdatePurchaseRequisitionCommandHandler : IRequestHandler<UpdatePurchaseRequisitionCommand, UpdatePurchaseRequisitionResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePurchaseRequisitionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdatePurchaseRequisitionResult> Handle(UpdatePurchaseRequisitionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PurchaseRequisition>();
        var requisition = await repository.Query()
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == request.Dto.Id, cancellationToken);

        if (requisition is null)
        {
            return UpdatePurchaseRequisitionResult.NotFound;
        }

        if (requisition.Status != RequisitionStatus.Draft)
        {
            return UpdatePurchaseRequisitionResult.Locked;
        }

        requisition.RequisitionNumber = request.Dto.RequisitionNumber;
        requisition.RequestDate = request.Dto.RequestDate;
        requisition.RequiredByDate = request.Dto.RequiredByDate;
        requisition.Description = request.Dto.Description;
        requisition.Status = request.Dto.Status;
        requisition.DepartmentId = request.Dto.DepartmentId;
        requisition.ProjectId = request.Dto.ProjectId;

        var detailRepository = _unitOfWork.Repository<PurchaseRequisitionDetail>();
        foreach (var detail in requisition.Details.ToList())
        {
            detailRepository.Remove(detail);
        }

        decimal estimatedAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var lineAmount = detail.Quantity * detail.EstimatedUnitPrice;
            await detailRepository.AddAsync(new PurchaseRequisitionDetail
            {
                PurchaseRequisitionId = requisition.Id,
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
        return UpdatePurchaseRequisitionResult.Success;
    }
}
