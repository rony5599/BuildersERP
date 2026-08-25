using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Suppliers;

public record SetSupplierActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetSupplierActiveCommandHandler : IRequestHandler<SetSupplierActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSupplierActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSupplierActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Supplier>();
        var supplier = await repository.GetByIdAsync(request.Id);
        if (supplier is null)
        {
            return false;
        }

        supplier.IsActive = request.IsActive;
        repository.Update(supplier);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
