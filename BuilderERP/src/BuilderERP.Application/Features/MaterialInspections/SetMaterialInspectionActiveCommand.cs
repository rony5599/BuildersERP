using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.MaterialInspections;

public record SetMaterialInspectionActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetMaterialInspectionActiveCommandHandler : IRequestHandler<SetMaterialInspectionActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetMaterialInspectionActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetMaterialInspectionActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<MaterialInspection>();
        var inspection = await repository.GetByIdAsync(request.Id);
        if (inspection is null)
        {
            return false;
        }

        inspection.IsActive = request.IsActive;
        repository.Update(inspection);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
