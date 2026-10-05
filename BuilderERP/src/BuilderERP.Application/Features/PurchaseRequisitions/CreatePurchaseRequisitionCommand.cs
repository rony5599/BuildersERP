using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using BuilderERP.Domain.Enums;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseRequisitions;

public record CreatePurchaseRequisitionCommand(CreatePurchaseRequisitionDto Dto) : IRequest<long>;

public class CreatePurchaseRequisitionCommandHandler : IRequestHandler<CreatePurchaseRequisitionCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreatePurchaseRequisitionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _numberGenerator = numberGenerator;
    }

    public async Task<long> Handle(CreatePurchaseRequisitionCommand request, CancellationToken cancellationToken)
    {
        var requisition = _mapper.Map<PurchaseRequisition>(request.Dto);
        requisition.Status = RequisitionStatus.Draft;
        requisition.RequisitionNumber = await _numberGenerator.GenerateAsync(request.Dto.ProjectId, "PR", cancellationToken);

        decimal estimatedAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var lineAmount = detail.Quantity * detail.EstimatedUnitPrice;
            requisition.Details.Add(new PurchaseRequisitionDetail
            {
                PurchaseRequisitionId = requisition.Id,
                MaterialId = detail.MaterialId,
                Quantity = detail.Quantity,
                UnitOfMeasure = detail.UnitOfMeasure,
                EstimatedUnitPrice = detail.EstimatedUnitPrice,
                EstimatedAmount = lineAmount,
                Remarks = detail.Remarks
            });
            estimatedAmount += lineAmount;
        }

        requisition.EstimatedAmount = estimatedAmount;

        await _unitOfWork.Repository<PurchaseRequisition>().AddAsync(requisition);
        await _unitOfWork.SaveChangesAsync();

        return requisition.Id;
    }
}
