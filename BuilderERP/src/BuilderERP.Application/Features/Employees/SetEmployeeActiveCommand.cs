using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Employees;

public record SetEmployeeActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetEmployeeActiveCommandHandler : IRequestHandler<SetEmployeeActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetEmployeeActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetEmployeeActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Employee>();
        var employee = await repository.GetByIdAsync(request.Id);
        if (employee is null)
        {
            return false;
        }

        employee.IsActive = request.IsActive;
        repository.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
