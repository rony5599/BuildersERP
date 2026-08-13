using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SnagItems;

public record SetSnagItemActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetSnagItemActiveCommandHandler : IRequestHandler<SetSnagItemActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSnagItemActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSnagItemActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SnagItem>();
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
