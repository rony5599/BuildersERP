using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DocumentVersions;

public record UpdateDocumentVersionCommand(UpdateDocumentVersionDto Dto) : IRequest<bool>;

public class UpdateDocumentVersionCommandHandler : IRequestHandler<UpdateDocumentVersionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDocumentVersionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDocumentVersionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<DocumentVersion>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.VersionNumber = request.Dto.VersionNumber;
        item.FilePath = request.Dto.FilePath;
        item.UploadedDate = request.Dto.UploadedDate;
        item.ChangeNotes = request.Dto.ChangeNotes;
        item.IsCurrent = request.Dto.IsCurrent;
        item.DocumentId = request.Dto.DocumentId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
