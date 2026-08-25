using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.EngineerWorkOrders;

public record CreateEngineerWorkOrderCommand(CreateEngineerWorkOrderDto Dto) : IRequest<Guid>;

public class CreateEngineerWorkOrderCommandHandler : IRequestHandler<CreateEngineerWorkOrderCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreateEngineerWorkOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _numberGenerator = numberGenerator;
    }

    public async Task<Guid> Handle(CreateEngineerWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var requisition = await _unitOfWork.Repository<EngineerWorkOrderRequisition>().GetByIdAsync(request.Dto.EngineerWorkOrderRequisitionId)
            ?? throw new InvalidOperationException("Engineer work order requisition not found.");

        var workOrder = _mapper.Map<EngineerWorkOrder>(request.Dto);
        workOrder.WorkOrderNo = await _numberGenerator.GenerateAsync(requisition.ProjectId, "EWO", cancellationToken);
        workOrder.RevisionNo = 0;
        workOrder.IsLatestRevision = true;
        workOrder.MotherWorkOrderId = null;
        workOrder.PreviousWorkOrderId = null;
        workOrder.RevisionDate = null;

        decimal totalAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var lineAmount = detail.Qty * detail.Rate;
            workOrder.Details.Add(new EngineerWorkOrderDetail
            {
                EngineerWorkOrderId = workOrder.Id,
                MaterialId = detail.MaterialId,
                UnitOfMeasure = detail.UnitOfMeasure,
                Qty = detail.Qty,
                Rate = detail.Rate,
                Amount = lineAmount,
                Remarks = detail.Remarks
            });
            totalAmount += lineAmount;
        }

        workOrder.TotalAmount = totalAmount;

        await _unitOfWork.Repository<EngineerWorkOrder>().AddAsync(workOrder);
        await _unitOfWork.SaveChangesAsync();

        return workOrder.Id;
    }
}
