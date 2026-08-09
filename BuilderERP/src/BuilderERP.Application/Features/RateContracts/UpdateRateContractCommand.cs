using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.RateContracts;

public record UpdateRateContractCommand(UpdateRateContractDto Dto) : IRequest<bool>;

public class UpdateRateContractCommandHandler : IRequestHandler<UpdateRateContractCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRateContractCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateRateContractCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<RateContract>();
        var contract = await repository.GetByIdAsync(request.Dto.Id);
        if (contract is null)
        {
            return false;
        }

        contract.ContractNumber = request.Dto.ContractNumber;
        contract.ItemDescription = request.Dto.ItemDescription;
        contract.UnitOfMeasure = request.Dto.UnitOfMeasure;
        contract.Rate = request.Dto.Rate;
        contract.EffectiveDate = request.Dto.EffectiveDate;
        contract.ExpiryDate = request.Dto.ExpiryDate;
        contract.ContractorId = request.Dto.ContractorId;
        contract.ProjectId = request.Dto.ProjectId;

        repository.Update(contract);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
