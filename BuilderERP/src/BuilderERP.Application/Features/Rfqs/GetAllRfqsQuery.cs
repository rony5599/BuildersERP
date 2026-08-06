using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Rfqs;

public record GetAllRfqsQuery : IRequest<IReadOnlyList<RfqDto>>;

public class GetAllRfqsQueryHandler : IRequestHandler<GetAllRfqsQuery, IReadOnlyList<RfqDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllRfqsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<RfqDto>> Handle(GetAllRfqsQuery request, CancellationToken cancellationToken)
    {
        var rfqs = await _unitOfWork.Repository<Rfq>().Query()
            .Include(r => r.PurchaseRequisition)
            .Include(r => r.Supplier)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<RfqDto>>(rfqs);
    }
}
