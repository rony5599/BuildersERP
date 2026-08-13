using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LandDocuments;

public record SetLandDocumentActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetLandDocumentActiveCommandHandler : IRequestHandler<SetLandDocumentActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetLandDocumentActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetLandDocumentActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<LandDocument>();
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
