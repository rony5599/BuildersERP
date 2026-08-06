using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Customers;

public record UpdateCustomerCommand(UpdateCustomerDto Dto) : IRequest<bool>;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Customer>();
        var customer = await repository.GetByIdAsync(request.Dto.Id);
        if (customer is null)
        {
            return false;
        }

        customer.FullName = request.Dto.FullName;
        customer.Email = request.Dto.Email;
        customer.Phone = request.Dto.Phone;
        customer.Address = request.Dto.Address;
        customer.NIDNumber = request.Dto.NIDNumber;
        customer.KycStatus = request.Dto.KycStatus;
        customer.CompanyId = request.Dto.CompanyId;
        customer.LeadId = request.Dto.LeadId;

        repository.Update(customer);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
