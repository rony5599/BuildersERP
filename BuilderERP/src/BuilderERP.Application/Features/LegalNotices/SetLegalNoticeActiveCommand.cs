using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LegalNotices;

public record SetLegalNoticeActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetLegalNoticeActiveCommandHandler : IRequestHandler<SetLegalNoticeActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetLegalNoticeActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetLegalNoticeActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<LegalNotice>();
        var item = await repository.GetByIdAsync(request.Id);
        if (item is null)
        {
            return false;
        }

        item.IsActive = request.IsActive;
        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
