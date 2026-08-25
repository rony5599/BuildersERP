using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Salaries;

public record SetSalaryActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetSalaryActiveCommandHandler : IRequestHandler<SetSalaryActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSalaryActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSalaryActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Salary>();
        var salary = await repository.GetByIdAsync(request.Id);
        if (salary is null)
        {
            return false;
        }

        salary.IsActive = request.IsActive;
        repository.Update(salary);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
