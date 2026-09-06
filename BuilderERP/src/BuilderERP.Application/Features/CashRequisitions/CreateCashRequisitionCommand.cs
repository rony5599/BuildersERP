using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CashRequisitions;

public record CreateCashRequisitionCommand(CreateCashRequisitionDto Dto) : IRequest<long>;

public class CreateCashRequisitionCommandHandler : IRequestHandler<CreateCashRequisitionCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentNumberGenerator _numberGenerator;

    public CreateCashRequisitionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDocumentNumberGenerator numberGenerator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _numberGenerator = numberGenerator;
    }

    public async Task<long> Handle(CreateCashRequisitionCommand request, CancellationToken cancellationToken)
    {
        var requisition = _mapper.Map<CashRequisition>(request.Dto);
        requisition.RequisitionNumber = await _numberGenerator.GenerateAsync(request.Dto.ProjectId, "CR", cancellationToken);

        decimal estimatedAmount = 0;
        foreach (var detail in request.Dto.Details)
        {
            var lineAmount = detail.Quantity * detail.EstimatedUnitPrice;
            requisition.Details.Add(new CashRequisitionDetail
            {
                CashRequisitionId = requisition.Id,
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

        await _unitOfWork.Repository<CashRequisition>().AddAsync(requisition);
        await _unitOfWork.SaveChangesAsync();

        return requisition.Id;
    }
}
