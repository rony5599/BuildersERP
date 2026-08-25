using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Workers;

public record SetWorkerActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetWorkerActiveCommandHandler : IRequestHandler<SetWorkerActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetWorkerActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetWorkerActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Worker>();
        var worker = await repository.GetByIdAsync(request.Id);
        if (worker is null)
        {
            return false;
        }

        worker.IsActive = request.IsActive;
        repository.Update(worker);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
