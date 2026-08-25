using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PunchLists;

public record SetPunchListActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetPunchListActiveCommandHandler : IRequestHandler<SetPunchListActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetPunchListActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetPunchListActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PunchList>();
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
