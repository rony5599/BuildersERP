using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using BuilderERP.Domain.Enums;
using MediatR;

namespace BuilderERP.Application.Features.EngineerWorkOrderRequisitions;

public record CreateEngineerWorkOrderRequisitionCommand(CreateEngineerWorkOrderRequisitionDto Dto) : IRequest<long>;

public class CreateEngineerWorkOrderRequisitionCommandHandler : IRequestHandler<CreateEngineerWorkOrderRequisitionCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreateEngineerWorkOrderRequisitionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _numberGenerator = numberGenerator;
    }

    public async Task<long> Handle(CreateEngineerWorkOrderRequisitionCommand request, CancellationToken cancellationToken)
    {
        var requisition = _mapper.Map<EngineerWorkOrderRequisition>(request.Dto);
        requisition.Status = RequisitionStatus.Draft;
        requisition.RequisitionNumber = await _numberGenerator.GenerateAsync(request.Dto.ProjectId, "EWR", cancellationToken);

        decimal estimatedAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var lineAmount = detail.Quantity * detail.EstimatedUnitPrice;
            requisition.Details.Add(new EngineerWorkOrderRequisitionDetail
            {
                EngineerWorkOrderRequisitionId = requisition.Id,
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

        await _unitOfWork.Repository<EngineerWorkOrderRequisition>().AddAsync(requisition);
        await _unitOfWork.SaveChangesAsync();

        return requisition.Id;
    }
}
