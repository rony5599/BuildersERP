using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.VisitorLogs;

public record SetVisitorLogActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetVisitorLogActiveCommandHandler : IRequestHandler<SetVisitorLogActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetVisitorLogActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetVisitorLogActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<VisitorLog>();
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
