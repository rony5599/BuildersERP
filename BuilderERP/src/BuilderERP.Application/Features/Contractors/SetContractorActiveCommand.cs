using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Contractors;

public record SetContractorActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetContractorActiveCommandHandler : IRequestHandler<SetContractorActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetContractorActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetContractorActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Contractor>();
        var contractor = await repository.GetByIdAsync(request.Id);
        if (contractor is null)
        {
            return false;
        }

        contractor.IsActive = request.IsActive;
        repository.Update(contractor);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
