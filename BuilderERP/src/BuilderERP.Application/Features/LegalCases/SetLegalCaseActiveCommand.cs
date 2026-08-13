using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LegalCases;

public record SetLegalCaseActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetLegalCaseActiveCommandHandler : IRequestHandler<SetLegalCaseActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetLegalCaseActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetLegalCaseActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<LegalCase>();
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
