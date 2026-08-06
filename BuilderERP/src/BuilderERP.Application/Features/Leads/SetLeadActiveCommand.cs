using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Leads;

public record SetLeadActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetLeadActiveCommandHandler : IRequestHandler<SetLeadActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetLeadActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetLeadActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Lead>();
        var lead = await repository.GetByIdAsync(request.Id);
        if (lead is null)
        {
            return false;
        }

        lead.IsActive = request.IsActive;
        repository.Update(lead);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
