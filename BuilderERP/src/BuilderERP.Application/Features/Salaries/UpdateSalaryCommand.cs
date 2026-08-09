using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Salaries;

public record UpdateSalaryCommand(UpdateSalaryDto Dto) : IRequest<bool>;

public class UpdateSalaryCommandHandler : IRequestHandler<UpdateSalaryCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSalaryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSalaryCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Salary>();
        var salary = await repository.GetByIdAsync(request.Dto.Id);
        if (salary is null)
        {
            return false;
        }

        salary.PeriodStart = request.Dto.PeriodStart;
        salary.PeriodEnd = request.Dto.PeriodEnd;
        salary.DaysWorked = request.Dto.DaysWorked;
        salary.BasicAmount = request.Dto.BasicAmount;
        salary.OvertimeAmount = request.Dto.OvertimeAmount;
        salary.DeductionAmount = request.Dto.DeductionAmount;
        salary.Status = request.Dto.Status;
        salary.PaymentDate = request.Dto.PaymentDate;
        salary.WorkerId = request.Dto.WorkerId;
        salary.NetAmount = salary.BasicAmount + salary.OvertimeAmount - salary.DeductionAmount;

        repository.Update(salary);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
