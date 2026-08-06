using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseRequisitions;

public record UpdatePurchaseRequisitionCommand(UpdatePurchaseRequisitionDto Dto) : IRequest<bool>;

public class UpdatePurchaseRequisitionCommandHandler : IRequestHandler<UpdatePurchaseRequisitionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePurchaseRequisitionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdatePurchaseRequisitionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PurchaseRequisition>();
        var requisition = await repository.GetByIdAsync(request.Dto.Id);
        if (requisition is null)
        {
            return false;
        }

        requisition.RequisitionNumber = request.Dto.RequisitionNumber;
        requisition.RequestDate = request.Dto.RequestDate;
        requisition.RequiredByDate = request.Dto.RequiredByDate;
        requisition.Description = request.Dto.Description;
        requisition.EstimatedAmount = request.Dto.EstimatedAmount;
        requisition.Status = request.Dto.Status;
        requisition.DepartmentId = request.Dto.DepartmentId;

        repository.Update(requisition);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
