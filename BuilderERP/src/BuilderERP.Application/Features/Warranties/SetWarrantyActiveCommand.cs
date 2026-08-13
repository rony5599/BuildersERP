using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Warranties;

public record SetWarrantyActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetWarrantyActiveCommandHandler : IRequestHandler<SetWarrantyActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetWarrantyActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetWarrantyActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Warranty>();
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
