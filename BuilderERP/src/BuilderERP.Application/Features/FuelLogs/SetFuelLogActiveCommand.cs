using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.FuelLogs;

public record SetFuelLogActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetFuelLogActiveCommandHandler : IRequestHandler<SetFuelLogActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetFuelLogActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetFuelLogActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<FuelLog>();
        var log = await repository.GetByIdAsync(request.Id);
        if (log is null)
        {
            return false;
        }

        log.IsActive = request.IsActive;
        repository.Update(log);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
