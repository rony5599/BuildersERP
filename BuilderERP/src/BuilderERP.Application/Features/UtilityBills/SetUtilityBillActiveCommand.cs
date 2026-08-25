using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.UtilityBills;

public record SetUtilityBillActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetUtilityBillActiveCommandHandler : IRequestHandler<SetUtilityBillActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetUtilityBillActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetUtilityBillActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<UtilityBill>();
        var item = await repository.GetByIdAsync(request.Id);
        if (item is null)
        {
            return false;
        }

        item.IsActive = request.IsActive;
        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
