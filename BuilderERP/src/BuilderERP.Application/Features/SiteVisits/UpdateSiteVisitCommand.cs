using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SiteVisits;

public record UpdateSiteVisitCommand(UpdateSiteVisitDto Dto) : IRequest<bool>;

public class UpdateSiteVisitCommandHandler : IRequestHandler<UpdateSiteVisitCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSiteVisitCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSiteVisitCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SiteVisit>();
        var visit = await repository.GetByIdAsync(request.Dto.Id);
        if (visit is null)
        {
            return false;
        }

        visit.VisitDate = request.Dto.VisitDate;
        visit.Status = request.Dto.Status;
        visit.Feedback = request.Dto.Feedback;
        visit.LeadId = request.Dto.LeadId;
        visit.PropertyUnitId = request.Dto.PropertyUnitId;
        visit.AssignedToUserId = request.Dto.AssignedToUserId;

        repository.Update(visit);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
