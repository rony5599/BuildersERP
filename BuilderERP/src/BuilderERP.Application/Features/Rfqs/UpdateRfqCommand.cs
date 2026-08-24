using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Rfqs;

public record UpdateRfqCommand(UpdateRfqDto Dto) : IRequest<UpdateRfqResult>;

public enum UpdateRfqResult
{
    Success,
    NotFound,
    Locked
}

public class UpdateRfqCommandHandler : IRequestHandler<UpdateRfqCommand, UpdateRfqResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRfqCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateRfqResult> Handle(UpdateRfqCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Rfq>();
        var rfq = await repository.Query()
            .Include(r => r.RfqVendors)
            .Include(r => r.Details)
            .FirstOrDefaultAsync(r => r.Id == request.Dto.Id, cancellationToken);

        if (rfq is null)
        {
            return UpdateRfqResult.NotFound;
        }

        if (rfq.Status == RfqStatus.Closed)
        {
            return UpdateRfqResult.Locked;
        }

        rfq.RfqNumber = request.Dto.RfqNumber;
        rfq.IssueDate = request.Dto.IssueDate;
        rfq.ClosingDate = request.Dto.ClosingDate;
        rfq.Status = request.Dto.Status;
        rfq.PurchaseRequisitionId = request.Dto.PurchaseRequisitionId;

        var vendorRepository = _unitOfWork.Repository<RfqVendor>();
        foreach (var vendor in rfq.RfqVendors.ToList())
        {
            vendorRepository.Remove(vendor);
        }

        foreach (var supplierId in request.Dto.SupplierIds.Distinct())
        {
            await vendorRepository.AddAsync(new RfqVendor { RfqId = rfq.Id, SupplierId = supplierId });
        }

        var detailRepository = _unitOfWork.Repository<RfqDetail>();
        foreach (var detail in rfq.Details.ToList())
        {
            detailRepository.Remove(detail);
        }

        foreach (var detail in request.Dto.Details)
        {
            await detailRepository.AddAsync(new RfqDetail
            {
                RfqId = rfq.Id,
                MaterialId = detail.MaterialId,
                Quantity = detail.Quantity,
                UnitOfMeasure = detail.UnitOfMeasure,
                Specification = detail.Specification
            });
        }

        repository.Update(rfq);
        await _unitOfWork.SaveChangesAsync();
        return UpdateRfqResult.Success;
    }
}
