using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Ncrs;

public record SetNcrActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetNcrActiveCommandHandler : IRequestHandler<SetNcrActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetNcrActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetNcrActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Ncr>();
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
