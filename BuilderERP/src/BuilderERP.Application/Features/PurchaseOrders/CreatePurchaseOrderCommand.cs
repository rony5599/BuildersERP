using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseOrders;

public record CreatePurchaseOrderCommand(CreatePurchaseOrderDto Dto) : IRequest<Guid>;

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePurchaseOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = _mapper.Map<PurchaseOrder>(request.Dto);
        await _unitOfWork.Repository<PurchaseOrder>().AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        return order.Id;
    }
}
