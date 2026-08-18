using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.WbsTasks;

public record UpdateWbsTaskCommand(UpdateWbsTaskDto Dto) : IRequest<bool>;

public class UpdateWbsTaskCommandHandler : IRequestHandler<UpdateWbsTaskCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWbsTaskCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateWbsTaskCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<WbsTask>();
        var wbsTask = await repository.GetByIdAsync(request.Dto.Id);
        if (wbsTask is null)
        {
            return false;
        }

        wbsTask.Code = request.Dto.Code;
        wbsTask.Name = request.Dto.Name;
        wbsTask.Description = request.Dto.Description;
        wbsTask.StartDate = request.Dto.StartDate;
        wbsTask.EndDate = request.Dto.EndDate;
        wbsTask.PercentComplete = request.Dto.PercentComplete;
        wbsTask.Sequence = request.Dto.Sequence;
        wbsTask.ParentId = request.Dto.ParentId;
        wbsTask.ProjectId = request.Dto.ProjectId;
        wbsTask.PropertyUnitId = request.Dto.PropertyUnitId;

        repository.Update(wbsTask);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
