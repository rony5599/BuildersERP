using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VendorQuotations;

public record GetVendorQuotationsByRequisitionIdQuery(Guid PurchaseRequisitionId) : IRequest<IReadOnlyList<VendorQuotationDto>>;

public class GetVendorQuotationsByRequisitionIdQueryHandler : IRequestHandler<GetVendorQuotationsByRequisitionIdQuery, IReadOnlyList<VendorQuotationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetVendorQuotationsByRequisitionIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<VendorQuotationDto>> Handle(GetVendorQuotationsByRequisitionIdQuery request, CancellationToken cancellationToken)
    {
        var quotations = await _unitOfWork.Repository<VendorQuotation>().Query()
            .Include(v => v.Supplier)
            .Include(v => v.Rfq)
            .ThenInclude(r => r.PurchaseRequisition)
            .Include(v => v.Details)
            .Where(v => v.Rfq.PurchaseRequisitionId == request.PurchaseRequisitionId)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<VendorQuotationDto>>(quotations);
    }
}
