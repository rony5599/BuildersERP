using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Quotations;

public record GetQuotationByIdQuery(Guid Id) : IRequest<QuotationDto?>;

public class GetQuotationByIdQueryHandler : IRequestHandler<GetQuotationByIdQuery, QuotationDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetQuotationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<QuotationDto?> Handle(GetQuotationByIdQuery request, CancellationToken cancellationToken)
    {
        var quotation = await _unitOfWork.Repository<Quotation>().GetByIdAsync(request.Id);
        return quotation is null ? null : _mapper.Map<QuotationDto>(quotation);
    }
}
