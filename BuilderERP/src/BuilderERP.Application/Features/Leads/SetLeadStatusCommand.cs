using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Leads;

public record SetLeadStatusCommand(Guid Id, LeadStatus Status) : IRequest<bool>;

public class SetLeadStatusCommandHandler : IRequestHandler<SetLeadStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetLeadStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetLeadStatusCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Lead>();
        var lead = await repository.GetByIdAsync(request.Id);
        if (lead is null)
        {
            return false;
        }

        lead.Status = request.Status;
        repository.Update(lead);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
