using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseRequisitions;

public record GetPurchaseRequisitionByIdQuery(Guid Id) : IRequest<PurchaseRequisitionDto?>;

public class GetPurchaseRequisitionByIdQueryHandler : IRequestHandler<GetPurchaseRequisitionByIdQuery, PurchaseRequisitionDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPurchaseRequisitionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PurchaseRequisitionDto?> Handle(GetPurchaseRequisitionByIdQuery request, CancellationToken cancellationToken)
    {
        var requisition = await _unitOfWork.Repository<PurchaseRequisition>().GetByIdAsync(request.Id);
        return requisition is null ? null : _mapper.Map<PurchaseRequisitionDto>(requisition);
    }
}
