using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DefectRecords;

public record UpdateDefectRecordCommand(UpdateDefectRecordDto Dto) : IRequest<bool>;

public class UpdateDefectRecordCommandHandler : IRequestHandler<UpdateDefectRecordCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDefectRecordCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDefectRecordCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<DefectRecord>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.DefectNumber = request.Dto.DefectNumber;
        item.Description = request.Dto.Description;
        item.Category = request.Dto.Category;
        item.Severity = request.Dto.Severity;
        item.Status = request.Dto.Status;
        item.ReportedDate = request.Dto.ReportedDate;
        item.ResolvedDate = request.Dto.ResolvedDate;
        item.AssignedTo = request.Dto.AssignedTo;
        item.Remarks = request.Dto.Remarks;
        item.PropertyUnitId = request.Dto.PropertyUnitId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
