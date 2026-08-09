using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Documents;

public record UpdateDocumentCommand(UpdateDocumentDto Dto) : IRequest<bool>;

public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDocumentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Document>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.DocumentNumber = request.Dto.DocumentNumber;
        item.Title = request.Dto.Title;
        item.DocumentType = request.Dto.DocumentType;
        item.FilePath = request.Dto.FilePath;
        item.IssueDate = request.Dto.IssueDate;
        item.ExpiryDate = request.Dto.ExpiryDate;
        item.Remarks = request.Dto.Remarks;
        item.CustomerId = request.Dto.CustomerId;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
