using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Commissions;

public record SetCommissionActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetCommissionActiveCommandHandler : IRequestHandler<SetCommissionActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetCommissionActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetCommissionActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Commission>();
        var commission = await repository.GetByIdAsync(request.Id);
        if (commission is null)
        {
            return false;
        }

        commission.IsActive = request.IsActive;
        repository.Update(commission);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
