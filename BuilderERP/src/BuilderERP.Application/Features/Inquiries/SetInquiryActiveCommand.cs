using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Inquiries;

public record SetInquiryActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetInquiryActiveCommandHandler : IRequestHandler<SetInquiryActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetInquiryActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetInquiryActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Inquiry>();
        var inquiry = await repository.GetByIdAsync(request.Id);
        if (inquiry is null)
        {
            return false;
        }

        inquiry.IsActive = request.IsActive;
        repository.Update(inquiry);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
