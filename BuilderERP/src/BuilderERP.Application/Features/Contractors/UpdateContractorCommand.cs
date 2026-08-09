using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Contractors;

public record UpdateContractorCommand(UpdateContractorDto Dto) : IRequest<bool>;

public class UpdateContractorCommandHandler : IRequestHandler<UpdateContractorCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContractorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateContractorCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Contractor>();
        var contractor = await repository.GetByIdAsync(request.Dto.Id);
        if (contractor is null)
        {
            return false;
        }

        contractor.ContractorCode = request.Dto.ContractorCode;
        contractor.Name = request.Dto.Name;
        contractor.ContactPerson = request.Dto.ContactPerson;
        contractor.Phone = request.Dto.Phone;
        contractor.Email = request.Dto.Email;
        contractor.Address = request.Dto.Address;
        contractor.LicenseNumber = request.Dto.LicenseNumber;
        contractor.Specialization = request.Dto.Specialization;

        repository.Update(contractor);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
