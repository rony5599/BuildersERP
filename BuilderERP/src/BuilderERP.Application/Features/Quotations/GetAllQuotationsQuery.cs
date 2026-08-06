using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Quotations;

public record GetAllQuotationsQuery : IRequest<IReadOnlyList<QuotationDto>>;

public class GetAllQuotationsQueryHandler : IRequestHandler<GetAllQuotationsQuery, IReadOnlyList<QuotationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllQuotationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<QuotationDto>> Handle(GetAllQuotationsQuery request, CancellationToken cancellationToken)
    {
        var quotations = await _unitOfWork.Repository<Quotation>().Query()
            .Include(q => q.Customer)
            .Include(q => q.PropertyUnit)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<QuotationDto>>(quotations);
    }
}
