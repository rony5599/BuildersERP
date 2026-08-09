using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DrawingRevisions;

public record SetDrawingRevisionActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetDrawingRevisionActiveCommandHandler : IRequestHandler<SetDrawingRevisionActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetDrawingRevisionActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetDrawingRevisionActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<DrawingRevision>();
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
