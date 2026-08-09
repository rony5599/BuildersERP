using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Drawings;

public record UpdateDrawingCommand(UpdateDrawingDto Dto) : IRequest<bool>;

public class UpdateDrawingCommandHandler : IRequestHandler<UpdateDrawingCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDrawingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDrawingCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Drawing>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.DrawingNumber = request.Dto.DrawingNumber;
        item.Title = request.Dto.Title;
        item.Discipline = request.Dto.Discipline;
        item.FilePath = request.Dto.FilePath;
        item.Status = request.Dto.Status;
        item.UploadedDate = request.Dto.UploadedDate;
        item.Remarks = request.Dto.Remarks;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
