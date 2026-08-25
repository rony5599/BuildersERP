using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.EngineerWorkOrderRequisitions;

public record SetEngineerWorkOrderRequisitionActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetEngineerWorkOrderRequisitionActiveCommandHandler : IRequestHandler<SetEngineerWorkOrderRequisitionActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetEngineerWorkOrderRequisitionActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetEngineerWorkOrderRequisitionActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<EngineerWorkOrderRequisition>();
        var requisition = await repository.GetByIdAsync(request.Id);
        if (requisition is null)
        {
            return false;
        }

        requisition.IsActive = request.IsActive;
        repository.Update(requisition);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
