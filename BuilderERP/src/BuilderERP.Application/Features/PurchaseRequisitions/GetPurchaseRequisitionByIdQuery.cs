using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseRequisitions;

public record GetPurchaseRequisitionByIdQuery(long Id) : IRequest<PurchaseRequisitionDto?>;

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
        var requisition = await _unitOfWork.Repository<PurchaseRequisition>().Query()
            .Include(r => r.Department)
            .Include(r => r.Project)
            .Include(r => r.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        return requisition is null ? null : _mapper.Map<PurchaseRequisitionDto>(requisition);
    }
}
