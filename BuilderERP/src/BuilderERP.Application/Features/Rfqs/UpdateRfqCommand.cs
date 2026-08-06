using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Rfqs;

public record UpdateRfqCommand(UpdateRfqDto Dto) : IRequest<bool>;

public class UpdateRfqCommandHandler : IRequestHandler<UpdateRfqCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRfqCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateRfqCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Rfq>();
        var rfq = await repository.GetByIdAsync(request.Dto.Id);
        if (rfq is null)
        {
            return false;
        }

        rfq.RfqNumber = request.Dto.RfqNumber;
        rfq.IssueDate = request.Dto.IssueDate;
        rfq.ClosingDate = request.Dto.ClosingDate;
        rfq.Status = request.Dto.Status;
        rfq.PurchaseRequisitionId = request.Dto.PurchaseRequisitionId;
        rfq.SupplierId = request.Dto.SupplierId;

        repository.Update(rfq);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
