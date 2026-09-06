using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Employees;

public record UpdateEmployeeCommand(UpdateEmployeeDto Dto) : IRequest<bool>;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Employee>();
        var employee = await repository.GetByIdAsync(request.Dto.Id);
        if (employee is null)
        {
            return false;
        }

        employee.EmployeeCode = request.Dto.EmployeeCode;
        employee.EmployeeName = request.Dto.EmployeeName;
        employee.MobileNo = request.Dto.MobileNo;
        employee.Email = request.Dto.Email;
        employee.DepartmentId = request.Dto.DepartmentId;
        employee.DesignationId = request.Dto.DesignationId;
        employee.BranchId = request.Dto.BranchId;
        employee.ReportingToId = request.Dto.ReportingToId;
        employee.UserId = request.Dto.UserId;

        repository.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
