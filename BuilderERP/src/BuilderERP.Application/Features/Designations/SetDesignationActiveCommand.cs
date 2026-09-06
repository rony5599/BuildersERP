using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Designations;

public record SetDesignationActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetDesignationActiveCommandHandler : IRequestHandler<SetDesignationActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetDesignationActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetDesignationActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Designation>();
        var designation = await repository.GetByIdAsync(request.Id);
        if (designation is null)
        {
            return false;
        }

        designation.IsActive = request.IsActive;
        repository.Update(designation);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
