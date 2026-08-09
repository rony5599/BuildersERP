using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.BoqItems;

public record SetBoqItemActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetBoqItemActiveCommandHandler : IRequestHandler<SetBoqItemActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetBoqItemActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetBoqItemActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<BoqItem>();
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
