using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SiteVisits;

public record SetSiteVisitActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetSiteVisitActiveCommandHandler : IRequestHandler<SetSiteVisitActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSiteVisitActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSiteVisitActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SiteVisit>();
        var visit = await repository.GetByIdAsync(request.Id);
        if (visit is null)
        {
            return false;
        }

        visit.IsActive = request.IsActive;
        repository.Update(visit);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
