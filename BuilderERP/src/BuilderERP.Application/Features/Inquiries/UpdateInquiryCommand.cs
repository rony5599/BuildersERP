using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Inquiries;

public record UpdateInquiryCommand(UpdateInquiryDto Dto) : IRequest<bool>;

public class UpdateInquiryCommandHandler : IRequestHandler<UpdateInquiryCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInquiryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateInquiryCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Inquiry>();
        var inquiry = await repository.GetByIdAsync(request.Dto.Id);
        if (inquiry is null)
        {
            return false;
        }

        inquiry.Message = request.Dto.Message;
        inquiry.InquiryDate = request.Dto.InquiryDate;
        inquiry.LeadId = request.Dto.LeadId;
        inquiry.PropertyUnitId = request.Dto.PropertyUnitId;

        repository.Update(inquiry);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
