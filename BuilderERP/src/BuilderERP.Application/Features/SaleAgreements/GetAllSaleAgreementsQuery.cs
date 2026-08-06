using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SaleAgreements;

public record GetAllSaleAgreementsQuery : IRequest<IReadOnlyList<SaleAgreementDto>>;

public class GetAllSaleAgreementsQueryHandler : IRequestHandler<GetAllSaleAgreementsQuery, IReadOnlyList<SaleAgreementDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSaleAgreementsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SaleAgreementDto>> Handle(GetAllSaleAgreementsQuery request, CancellationToken cancellationToken)
    {
        var agreements = await _unitOfWork.Repository<SaleAgreement>().Query()
            .Include(a => a.Booking)
            .ThenInclude(b => b.PropertyUnit)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SaleAgreementDto>>(agreements);
    }
}
