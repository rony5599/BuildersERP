using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseReturns;

public record CreatePurchaseReturnCommand(CreatePurchaseReturnDto Dto) : IRequest<Guid>;

public class CreatePurchaseReturnCommandHandler : IRequestHandler<CreatePurchaseReturnCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePurchaseReturnCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreatePurchaseReturnCommand request, CancellationToken cancellationToken)
    {
        var purchaseReturn = _mapper.Map<PurchaseReturn>(request.Dto);
        await _unitOfWork.Repository<PurchaseReturn>().AddAsync(purchaseReturn);
        await _unitOfWork.SaveChangesAsync();
        return purchaseReturn.Id;
    }
}
