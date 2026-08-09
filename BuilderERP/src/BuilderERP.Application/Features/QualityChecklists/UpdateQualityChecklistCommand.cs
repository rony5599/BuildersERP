using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.QualityChecklists;

public record UpdateQualityChecklistCommand(UpdateQualityChecklistDto Dto) : IRequest<bool>;

public class UpdateQualityChecklistCommandHandler : IRequestHandler<UpdateQualityChecklistCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateQualityChecklistCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateQualityChecklistCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<QualityChecklist>();
        var checklist = await repository.GetByIdAsync(request.Dto.Id);
        if (checklist is null)
        {
            return false;
        }

        checklist.ProjectId = request.Dto.ProjectId;
        checklist.ChecklistName = request.Dto.ChecklistName;
        checklist.Category = request.Dto.Category;
        checklist.ChecklistDate = request.Dto.ChecklistDate;
        checklist.CheckedBy = request.Dto.CheckedBy;
        checklist.Result = request.Dto.Result;
        checklist.Remarks = request.Dto.Remarks;

        repository.Update(checklist);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
