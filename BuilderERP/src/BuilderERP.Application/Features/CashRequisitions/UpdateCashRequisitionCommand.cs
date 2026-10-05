using BuilderERP.Application.Common.Caching;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.CashRequisitions;

public record UpdateCashRequisitionCommand(UpdateCashRequisitionDto Dto) : IRequest<UpdateCashRequisitionResult>, IInvalidatesFeatures
{
    // Also read by other features' cached queries: the requester ledger and the disbursable-requisition dropdown are cached under RequesterLedger and CashDisbursements.
    public IReadOnlyCollection<string> AdditionalFeatures { get; } = ["RequesterLedger", "CashDisbursements"];
}

public enum UpdateCashRequisitionResult
{
    Success,
    NotFound,
    Locked
}

public class UpdateCashRequisitionCommandHandler : IRequestHandler<UpdateCashRequisitionCommand, UpdateCashRequisitionResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCashRequisitionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCashRequisitionResult> Handle(UpdateCashRequisitionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CashRequisition>();
        var requisition = await repository.Query()
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == request.Dto.Id, cancellationToken);

        if (requisition is null)
        {
            return UpdateCashRequisitionResult.NotFound;
        }

        if (requisition.Status != RequisitionStatus.Draft)
        {
            return UpdateCashRequisitionResult.Locked;
        }

        requisition.RequisitionNumber = request.Dto.RequisitionNumber;
        requisition.RequestDate = request.Dto.RequestDate;
        requisition.RequiredByDate = request.Dto.RequiredByDate;
        requisition.Description = request.Dto.Description;
        requisition.Status = request.Dto.Status;
        requisition.RequesterEmployeeId = request.Dto.RequesterEmployeeId;
        requisition.PaymentMethod = request.Dto.PaymentMethod;
        requisition.DepartmentId = request.Dto.DepartmentId;
        requisition.ProjectId = request.Dto.ProjectId;

        var detailRepository = _unitOfWork.Repository<CashRequisitionDetail>();
        foreach (var detail in requisition.Details.ToList())
        {
            detailRepository.Remove(detail);
        }

        decimal estimatedAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var lineAmount = detail.Quantity * detail.EstimatedUnitPrice;
            await detailRepository.AddAsync(new CashRequisitionDetail
            {
                CashRequisitionId = requisition.Id,
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
        return UpdateCashRequisitionResult.Success;
    }
}
