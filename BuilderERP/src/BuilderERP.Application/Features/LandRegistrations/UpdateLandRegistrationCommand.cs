using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LandRegistrations;

public record UpdateLandRegistrationCommand(UpdateLandRegistrationDto Dto) : IRequest<bool>;

public class UpdateLandRegistrationCommandHandler : IRequestHandler<UpdateLandRegistrationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLandRegistrationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateLandRegistrationCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<LandRegistration>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.RegistrationNumber = request.Dto.RegistrationNumber;
        item.DeedNumber = request.Dto.DeedNumber;
        item.RegistrationDate = request.Dto.RegistrationDate;
        item.SubRegistryOffice = request.Dto.SubRegistryOffice;
        item.RegistrationFee = request.Dto.RegistrationFee;
        item.Status = request.Dto.Status;
        item.Remarks = request.Dto.Remarks;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
