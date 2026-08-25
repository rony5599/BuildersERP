using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Rfqs;

public record CreateRfqCommand(CreateRfqDto Dto) : IRequest<long>;

public class CreateRfqCommandHandler : IRequestHandler<CreateRfqCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreateRfqCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _numberGenerator = numberGenerator;
    }

    public async Task<long> Handle(CreateRfqCommand request, CancellationToken cancellationToken)
    {
        var projectId = await _unitOfWork.Repository<PurchaseRequisition>().Query()
            .Where(r => r.Id == request.Dto.PurchaseRequisitionId)
            .Select(r => r.ProjectId)
            .SingleAsync(cancellationToken);

        var rfq = _mapper.Map<Rfq>(request.Dto);
        rfq.RfqNumber = await _numberGenerator.GenerateAsync(projectId, "RFQ", cancellationToken);

        foreach (var supplierId in request.Dto.SupplierIds.Distinct())
        {
            rfq.RfqVendors.Add(new RfqVendor { RfqId = rfq.Id, SupplierId = supplierId });
        }

        foreach (var detail in request.Dto.Details)
        {
            rfq.Details.Add(new RfqDetail
            {
                RfqId = rfq.Id,
                MaterialId = detail.MaterialId,
                Quantity = detail.Quantity,
                UnitOfMeasure = detail.UnitOfMeasure,
                Specification = detail.Specification
            });
        }

        await _unitOfWork.Repository<Rfq>().AddAsync(rfq);
        await _unitOfWork.SaveChangesAsync();

        return rfq.Id;
    }
}
