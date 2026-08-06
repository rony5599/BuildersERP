using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using BuilderERP.Application.DTOs;
using MediatR;

namespace BuilderERP.Application.Features.CostCenters;

public record UpdateCostCenterCommand(UpdateCostCenterDto Dto) : IRequest<bool>;

public class UpdateCostCenterCommandHandler : IRequestHandler<UpdateCostCenterCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCostCenterCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCostCenterCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CostCenter>();
        var costCenter = await repository.GetByIdAsync(request.Dto.Id);
        if (costCenter is null)
        {
            return false;
        }

        costCenter.Name = request.Dto.Name;
        costCenter.Code = request.Dto.Code;
        costCenter.ProjectId = request.Dto.ProjectId;

        repository.Update(costCenter);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
