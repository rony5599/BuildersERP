using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.FollowUps;

public record UpdateFollowUpCommand(UpdateFollowUpDto Dto) : IRequest<bool>;

public class UpdateFollowUpCommandHandler : IRequestHandler<UpdateFollowUpCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFollowUpCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateFollowUpCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<FollowUp>();
        var followUp = await repository.GetByIdAsync(request.Dto.Id);
        if (followUp is null)
        {
            return false;
        }

        followUp.Notes = request.Dto.Notes;
        followUp.FollowUpDate = request.Dto.FollowUpDate;
        followUp.NextFollowUpDate = request.Dto.NextFollowUpDate;
        followUp.Outcome = request.Dto.Outcome;
        followUp.LeadId = request.Dto.LeadId;

        repository.Update(followUp);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
