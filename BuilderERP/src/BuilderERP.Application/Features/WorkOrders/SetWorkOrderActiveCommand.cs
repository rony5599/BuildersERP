using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.WorkOrders;

public record SetWorkOrderActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetWorkOrderActiveCommandHandler : IRequestHandler<SetWorkOrderActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetWorkOrderActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetWorkOrderActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<WorkOrder>();
        var workOrder = await repository.GetByIdAsync(request.Id);
        if (workOrder is null)
        {
            return false;
        }

        workOrder.IsActive = request.IsActive;
        repository.Update(workOrder);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
