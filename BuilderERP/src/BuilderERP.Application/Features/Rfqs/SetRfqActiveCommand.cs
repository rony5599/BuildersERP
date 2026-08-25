using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Rfqs;

public record SetRfqActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetRfqActiveCommandHandler : IRequestHandler<SetRfqActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetRfqActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetRfqActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Rfq>();
        var rfq = await repository.GetByIdAsync(request.Id);
        if (rfq is null)
        {
            return false;
        }

        rfq.IsActive = request.IsActive;
        repository.Update(rfq);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
