using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SaleAgreements;

public record GetSaleAgreementByIdQuery(long Id) : IRequest<SaleAgreementDto?>;

public class GetSaleAgreementByIdQueryHandler : IRequestHandler<GetSaleAgreementByIdQuery, SaleAgreementDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSaleAgreementByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SaleAgreementDto?> Handle(GetSaleAgreementByIdQuery request, CancellationToken cancellationToken)
    {
        var agreement = await _unitOfWork.Repository<SaleAgreement>().GetByIdAsync(request.Id);
        return agreement is null ? null : _mapper.Map<SaleAgreementDto>(agreement);
    }
}
