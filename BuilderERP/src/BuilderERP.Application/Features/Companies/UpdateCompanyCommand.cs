using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using BuilderERP.Application.DTOs;
using MediatR;

namespace BuilderERP.Application.Features.Companies;

public record UpdateCompanyCommand(UpdateCompanyDto Dto) : IRequest<bool>;

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Company>();
        var company = await repository.GetByIdAsync(request.Dto.Id);
        if (company is null)
        {
            return false;
        }

        company.Name = request.Dto.Name;
        company.Code = request.Dto.Code;
        company.RegistrationNumber = request.Dto.RegistrationNumber;
        company.Address = request.Dto.Address;
        company.Phone = request.Dto.Phone;
        company.Email = request.Dto.Email;

        repository.Update(company);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
