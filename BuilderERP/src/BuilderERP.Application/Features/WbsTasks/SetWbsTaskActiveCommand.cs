using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.WbsTasks;

public record SetWbsTaskActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetWbsTaskActiveCommandHandler : IRequestHandler<SetWbsTaskActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetWbsTaskActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetWbsTaskActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<WbsTask>();
        var wbsTask = await repository.GetByIdAsync(request.Id);
        if (wbsTask is null)
        {
            return false;
        }

        wbsTask.IsActive = request.IsActive;
        repository.Update(wbsTask);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
