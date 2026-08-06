using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VendorQuotations;

public record GetAllVendorQuotationsQuery : IRequest<IReadOnlyList<VendorQuotationDto>>;

public class GetAllVendorQuotationsQueryHandler : IRequestHandler<GetAllVendorQuotationsQuery, IReadOnlyList<VendorQuotationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllVendorQuotationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<VendorQuotationDto>> Handle(GetAllVendorQuotationsQuery request, CancellationToken cancellationToken)
    {
        var quotations = await _unitOfWork.Repository<VendorQuotation>().Query()
            .Include(v => v.Rfq)
            .ThenInclude(r => r.Supplier)
            .Include(v => v.Rfq)
            .ThenInclude(r => r.PurchaseRequisition)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<VendorQuotationDto>>(quotations);
    }
}
