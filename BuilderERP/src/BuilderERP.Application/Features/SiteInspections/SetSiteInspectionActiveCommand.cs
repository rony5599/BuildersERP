using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SiteInspections;

public record SetSiteInspectionActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetSiteInspectionActiveCommandHandler : IRequestHandler<SetSiteInspectionActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSiteInspectionActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSiteInspectionActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SiteInspection>();
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
