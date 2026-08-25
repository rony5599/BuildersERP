using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.GoodsReceives;

public record SetGoodsReceiveActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetGoodsReceiveActiveCommandHandler : IRequestHandler<SetGoodsReceiveActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetGoodsReceiveActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetGoodsReceiveActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<GoodsReceive>();
        var receive = await repository.GetByIdAsync(request.Id);
        if (receive is null)
        {
            return false;
        }

        receive.IsActive = request.IsActive;
        repository.Update(receive);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
