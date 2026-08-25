using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Customers;

public record SetCustomerActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetCustomerActiveCommandHandler : IRequestHandler<SetCustomerActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetCustomerActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetCustomerActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Customer>();
        var customer = await repository.GetByIdAsync(request.Id);
        if (customer is null)
        {
            return false;
        }

        customer.IsActive = request.IsActive;
        repository.Update(customer);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
