using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PpeTrackings;

public record SetPpeTrackingActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetPpeTrackingActiveCommandHandler : IRequestHandler<SetPpeTrackingActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetPpeTrackingActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetPpeTrackingActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PpeTracking>();
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
