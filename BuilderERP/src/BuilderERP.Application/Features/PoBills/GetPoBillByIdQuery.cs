using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PoBills;

public record GetPoBillByIdQuery(long Id) : IRequest<PoBillDto?>;

public class GetPoBillByIdQueryHandler : IRequestHandler<GetPoBillByIdQuery, PoBillDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPoBillByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PoBillDto?> Handle(GetPoBillByIdQuery request, CancellationToken cancellationToken)
    {
        var bill = await _unitOfWork.Repository<PoBill>().Query()
            .Include(b => b.PurchaseOrder).ThenInclude(o => o.VendorQuotation).ThenInclude(v => v.Supplier)
            .Include(b => b.PurchaseOrder).ThenInclude(o => o.VendorQuotation).ThenInclude(v => v.Rfq).ThenInclude(r => r.PurchaseRequisition).ThenInclude(pr => pr.Project)
            .Include(b => b.Details).ThenInclude(d => d.Material)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        return bill is null ? null : _mapper.Map<PoBillDto>(bill);
    }
}
