using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VendorQuotations;

public record SelectWinnerVendorQuotationCommand(Guid Id) : IRequest<bool>;

public class SelectWinnerVendorQuotationCommandHandler : IRequestHandler<SelectWinnerVendorQuotationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SelectWinnerVendorQuotationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SelectWinnerVendorQuotationCommand request, CancellationToken cancellationToken)
    {
        var quotationRepository = _unitOfWork.Repository<VendorQuotation>();
        var winner = await quotationRepository.Query()
            .Include(v => v.Rfq)
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);
        if (winner is null)
        {
            return false;
        }

        var purchaseRequisitionId = winner.Rfq.PurchaseRequisitionId;

        var siblings = await quotationRepository.Query()
            .Include(v => v.Rfq)
            .Where(v => v.Rfq.PurchaseRequisitionId == purchaseRequisitionId && v.Id != winner.Id)
            .ToListAsync(cancellationToken);

        winner.Status = VendorQuotationStatus.Selected;
        quotationRepository.Update(winner);

        foreach (var sibling in siblings)
        {
            sibling.Status = VendorQuotationStatus.Rejected;
            quotationRepository.Update(sibling);
        }

        var requisitionRepository = _unitOfWork.Repository<PurchaseRequisition>();
        var requisition = await requisitionRepository.GetByIdAsync(purchaseRequisitionId);
        if (requisition is not null)
        {
            requisition.Status = RequisitionStatus.Converted;
            requisitionRepository.Update(requisition);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
