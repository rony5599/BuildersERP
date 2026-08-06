using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using BuilderERP.Application.DTOs;
using MediatR;

namespace BuilderERP.Application.Features.Departments;

public record UpdateDepartmentCommand(UpdateDepartmentDto Dto) : IRequest<bool>;

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Department>();
        var department = await repository.GetByIdAsync(request.Dto.Id);
        if (department is null)
        {
            return false;
        }

        department.Name = request.Dto.Name;
        department.Code = request.Dto.Code;
        department.BranchId = request.Dto.BranchId;

        repository.Update(department);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
