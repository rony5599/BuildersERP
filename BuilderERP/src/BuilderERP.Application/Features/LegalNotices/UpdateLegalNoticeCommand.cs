using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LegalNotices;

public record UpdateLegalNoticeCommand(UpdateLegalNoticeDto Dto) : IRequest<bool>;

public class UpdateLegalNoticeCommandHandler : IRequestHandler<UpdateLegalNoticeCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLegalNoticeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateLegalNoticeCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<LegalNotice>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.NoticeNumber = request.Dto.NoticeNumber;
        item.Title = request.Dto.Title;
        item.NoticeType = request.Dto.NoticeType;
        item.IssuedTo = request.Dto.IssuedTo;
        item.IssueDate = request.Dto.IssueDate;
        item.ResponseDeadline = request.Dto.ResponseDeadline;
        item.Status = request.Dto.Status;
        item.Remarks = request.Dto.Remarks;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
