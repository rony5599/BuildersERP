using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PurchaseOrders;

public record UpdatePurchaseOrderCommand(UpdatePurchaseOrderDto Dto) : IRequest<bool>;

public class UpdatePurchaseOrderCommandHandler : IRequestHandler<UpdatePurchaseOrderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePurchaseOrderCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PurchaseOrder>();
        var order = await repository.GetByIdAsync(request.Dto.Id);
        if (order is null)
        {
            return false;
        }

        order.PONumber = request.Dto.PONumber;
        order.OrderDate = request.Dto.OrderDate;
        order.TotalAmount = request.Dto.TotalAmount;
        order.DeliveryDate = request.Dto.DeliveryDate;
        order.Status = request.Dto.Status;
        order.VendorQuotationId = request.Dto.VendorQuotationId;

        repository.Update(order);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
