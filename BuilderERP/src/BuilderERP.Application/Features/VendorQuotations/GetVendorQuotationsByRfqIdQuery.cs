using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.VendorQuotations;

public record GetVendorQuotationsByRfqIdQuery(long RfqId) : IRequest<IReadOnlyList<VendorQuotationDto>>;

public class GetVendorQuotationsByRfqIdQueryHandler
    : IRequestHandler<GetVendorQuotationsByRfqIdQuery, IReadOnlyList<VendorQuotationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetVendorQuotationsByRfqIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<VendorQuotationDto>> Handle(
        GetVendorQuotationsByRfqIdQuery request,
        CancellationToken cancellationToken)
    {
        var quotations = await _unitOfWork.Repository<VendorQuotation>().Query()
            .AsNoTracking()
            .Include(q => q.Supplier)
            .Include(q => q.Rfq)
                .ThenInclude(r => r.PurchaseRequisition)
                .ThenInclude(pr => pr.Project)
            .Include(q => q.Details)
                .ThenInclude(d => d.Material)
            .Where(q => q.RfqId == request.RfqId && q.IsActive)
            .OrderBy(q => q.QuotedAmount)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<VendorQuotationDto>>(quotations);
    }
}
