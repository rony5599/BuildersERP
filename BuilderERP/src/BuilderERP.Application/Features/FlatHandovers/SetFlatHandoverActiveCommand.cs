using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.FlatHandovers;

public record SetFlatHandoverActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetFlatHandoverActiveCommandHandler : IRequestHandler<SetFlatHandoverActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetFlatHandoverActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetFlatHandoverActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<FlatHandover>();
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
