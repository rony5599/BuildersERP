using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DocumentVersions;

public record SetDocumentVersionActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetDocumentVersionActiveCommandHandler : IRequestHandler<SetDocumentVersionActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetDocumentVersionActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetDocumentVersionActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<DocumentVersion>();
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
