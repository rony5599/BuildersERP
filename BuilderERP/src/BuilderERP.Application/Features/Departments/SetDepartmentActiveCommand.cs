using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Departments;

public record SetDepartmentActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetDepartmentActiveCommandHandler : IRequestHandler<SetDepartmentActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetDepartmentActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetDepartmentActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Department>();
        var department = await repository.GetByIdAsync(request.Id);
        if (department is null)
        {
            return false;
        }

        department.IsActive = request.IsActive;
        repository.Update(department);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
