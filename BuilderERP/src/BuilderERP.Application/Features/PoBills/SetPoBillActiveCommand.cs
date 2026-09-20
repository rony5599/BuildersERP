using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PoBills;

public record SetPoBillActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetPoBillActiveCommandHandler : IRequestHandler<SetPoBillActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetPoBillActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetPoBillActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PoBill>();
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
