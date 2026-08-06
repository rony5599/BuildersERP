using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseReturns;

public record GetPurchaseReturnByIdQuery(Guid Id) : IRequest<PurchaseReturnDto?>;

public class GetPurchaseReturnByIdQueryHandler : IRequestHandler<GetPurchaseReturnByIdQuery, PurchaseReturnDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPurchaseReturnByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PurchaseReturnDto?> Handle(GetPurchaseReturnByIdQuery request, CancellationToken cancellationToken)
    {
        var purchaseReturn = await _unitOfWork.Repository<PurchaseReturn>().GetByIdAsync(request.Id);
        return purchaseReturn is null ? null : _mapper.Map<PurchaseReturnDto>(purchaseReturn);
    }
}
