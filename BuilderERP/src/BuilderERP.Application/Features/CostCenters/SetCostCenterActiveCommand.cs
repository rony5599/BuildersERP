using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CostCenters;

public record SetCostCenterActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetCostCenterActiveCommandHandler : IRequestHandler<SetCostCenterActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetCostCenterActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetCostCenterActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CostCenter>();
        var costCenter = await repository.GetByIdAsync(request.Id);
        if (costCenter is null)
        {
            return false;
        }

        costCenter.IsActive = request.IsActive;
        repository.Update(costCenter);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
