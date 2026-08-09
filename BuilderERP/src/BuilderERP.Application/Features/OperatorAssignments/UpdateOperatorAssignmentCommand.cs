using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.OperatorAssignments;

public record UpdateOperatorAssignmentCommand(UpdateOperatorAssignmentDto Dto) : IRequest<bool>;

public class UpdateOperatorAssignmentCommandHandler : IRequestHandler<UpdateOperatorAssignmentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOperatorAssignmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateOperatorAssignmentCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<OperatorAssignment>();
        var assignment = await repository.GetByIdAsync(request.Dto.Id);
        if (assignment is null)
        {
            return false;
        }

        assignment.AssignmentStartDate = request.Dto.AssignmentStartDate;
        assignment.AssignmentEndDate = request.Dto.AssignmentEndDate;
        assignment.EquipmentId = request.Dto.EquipmentId;
        assignment.WorkerId = request.Dto.WorkerId;
        assignment.ProjectId = request.Dto.ProjectId;

        repository.Update(assignment);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
