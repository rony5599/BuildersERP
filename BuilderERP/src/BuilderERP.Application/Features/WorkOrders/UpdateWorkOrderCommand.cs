using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.WorkOrders;

public record UpdateWorkOrderCommand(UpdateWorkOrderDto Dto) : IRequest<bool>;

public class UpdateWorkOrderCommandHandler : IRequestHandler<UpdateWorkOrderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWorkOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<WorkOrder>();
        var workOrder = await repository.GetByIdAsync(request.Dto.Id);
        if (workOrder is null)
        {
            return false;
        }

        workOrder.WorkOrderNumber = request.Dto.WorkOrderNumber;
        workOrder.Description = request.Dto.Description;
        workOrder.OrderDate = request.Dto.OrderDate;
        workOrder.CompletionDate = request.Dto.CompletionDate;
        workOrder.Amount = request.Dto.Amount;
        workOrder.Status = request.Dto.Status;
        workOrder.ContractorId = request.Dto.ContractorId;
        workOrder.ProjectId = request.Dto.ProjectId;

        repository.Update(workOrder);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
