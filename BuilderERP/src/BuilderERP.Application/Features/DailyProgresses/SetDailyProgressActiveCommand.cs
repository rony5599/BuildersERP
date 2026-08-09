using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DailyProgresses;

public record SetDailyProgressActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetDailyProgressActiveCommandHandler : IRequestHandler<SetDailyProgressActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetDailyProgressActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetDailyProgressActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<DailyProgress>();
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
