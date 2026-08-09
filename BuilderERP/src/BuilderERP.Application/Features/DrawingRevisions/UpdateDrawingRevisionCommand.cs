using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DrawingRevisions;

public record UpdateDrawingRevisionCommand(UpdateDrawingRevisionDto Dto) : IRequest<bool>;

public class UpdateDrawingRevisionCommandHandler : IRequestHandler<UpdateDrawingRevisionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDrawingRevisionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDrawingRevisionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<DrawingRevision>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.RevisionCode = request.Dto.RevisionCode;
        item.FilePath = request.Dto.FilePath;
        item.RevisedDate = request.Dto.RevisedDate;
        item.ChangeDescription = request.Dto.ChangeDescription;
        item.IsCurrent = request.Dto.IsCurrent;
        item.DrawingId = request.Dto.DrawingId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
