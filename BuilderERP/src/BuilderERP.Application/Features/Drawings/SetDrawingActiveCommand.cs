using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Drawings;

public record SetDrawingActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetDrawingActiveCommandHandler : IRequestHandler<SetDrawingActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetDrawingActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetDrawingActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Drawing>();
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
