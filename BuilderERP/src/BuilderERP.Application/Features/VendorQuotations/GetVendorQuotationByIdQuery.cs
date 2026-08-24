using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VendorQuotations;

public record GetVendorQuotationByIdQuery(Guid Id) : IRequest<VendorQuotationDto?>;

public class GetVendorQuotationByIdQueryHandler : IRequestHandler<GetVendorQuotationByIdQuery, VendorQuotationDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetVendorQuotationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<VendorQuotationDto?> Handle(GetVendorQuotationByIdQuery request, CancellationToken cancellationToken)
    {
        var quotation = await _unitOfWork.Repository<VendorQuotation>().Query()
            .Include(v => v.Supplier)
            .Include(v => v.Rfq).ThenInclude(r => r.PurchaseRequisition).ThenInclude(pr => pr.Project)
            .Include(v => v.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);
        return quotation is null ? null : _mapper.Map<VendorQuotationDto>(quotation);
    }
}
