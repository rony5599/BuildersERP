using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Documents;

public record SetDocumentActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetDocumentActiveCommandHandler : IRequestHandler<SetDocumentActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetDocumentActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetDocumentActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Document>();
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
