using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.BoqItems;

public record UpdateBoqItemCommand(UpdateBoqItemDto Dto) : IRequest<bool>;

public class UpdateBoqItemCommandHandler : IRequestHandler<UpdateBoqItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBoqItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateBoqItemCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<BoqItem>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.Description = request.Dto.Description;
        item.UnitOfMeasure = request.Dto.UnitOfMeasure;
        item.Quantity = request.Dto.Quantity;
        item.Rate = request.Dto.Rate;
        item.BoqId = request.Dto.BoqId;
        item.WorkGroupId = request.Dto.WorkGroupId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
