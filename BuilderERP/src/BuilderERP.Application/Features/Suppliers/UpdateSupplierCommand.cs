using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Suppliers;

public record UpdateSupplierCommand(UpdateSupplierDto Dto) : IRequest<bool>;

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSupplierCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Supplier>();
        var supplier = await repository.GetByIdAsync(request.Dto.Id);
        if (supplier is null)
        {
            return false;
        }

        supplier.SupplierCode = request.Dto.SupplierCode;
        supplier.Name = request.Dto.Name;
        supplier.ContactPerson = request.Dto.ContactPerson;
        supplier.Phone = request.Dto.Phone;
        supplier.Email = request.Dto.Email;
        supplier.Address = request.Dto.Address;

        repository.Update(supplier);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
