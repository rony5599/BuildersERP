using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Overtimes;

public record SetOvertimeActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetOvertimeActiveCommandHandler : IRequestHandler<SetOvertimeActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetOvertimeActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetOvertimeActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Overtime>();
        var overtime = await repository.GetByIdAsync(request.Id);
        if (overtime is null)
        {
            return false;
        }

        overtime.IsActive = request.IsActive;
        repository.Update(overtime);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
