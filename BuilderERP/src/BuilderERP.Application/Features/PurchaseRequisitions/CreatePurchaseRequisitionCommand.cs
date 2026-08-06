using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseRequisitions;

public record CreatePurchaseRequisitionCommand(CreatePurchaseRequisitionDto Dto) : IRequest<Guid>;

public class CreatePurchaseRequisitionCommandHandler : IRequestHandler<CreatePurchaseRequisitionCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePurchaseRequisitionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreatePurchaseRequisitionCommand request, CancellationToken cancellationToken)
    {
        var requisition = _mapper.Map<PurchaseRequisition>(request.Dto);
        await _unitOfWork.Repository<PurchaseRequisition>().AddAsync(requisition);
        await _unitOfWork.SaveChangesAsync();

        return requisition.Id;
    }
}
