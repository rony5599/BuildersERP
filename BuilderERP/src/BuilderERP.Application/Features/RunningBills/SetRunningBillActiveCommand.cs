using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RunningBills;

public record SetRunningBillActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetRunningBillActiveCommandHandler : IRequestHandler<SetRunningBillActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetRunningBillActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetRunningBillActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<RunningBill>();
        var bill = await repository.GetByIdAsync(request.Id);
        if (bill is null)
        {
            return false;
        }

        bill.IsActive = request.IsActive;
        repository.Update(bill);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
