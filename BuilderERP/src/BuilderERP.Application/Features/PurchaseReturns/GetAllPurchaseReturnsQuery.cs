using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PurchaseReturns;

public record GetAllPurchaseReturnsQuery : IRequest<IReadOnlyList<PurchaseReturnDto>>;

public class GetAllPurchaseReturnsQueryHandler : IRequestHandler<GetAllPurchaseReturnsQuery, IReadOnlyList<PurchaseReturnDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPurchaseReturnsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PurchaseReturnDto>> Handle(GetAllPurchaseReturnsQuery request, CancellationToken cancellationToken)
    {
        var returns = await _unitOfWork.Repository<PurchaseReturn>().Query()
            .Include(r => r.GoodsReceive)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PurchaseReturnDto>>(returns);
    }
}
