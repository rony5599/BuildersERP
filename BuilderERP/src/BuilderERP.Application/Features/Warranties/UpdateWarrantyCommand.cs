using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Warranties;

public record UpdateWarrantyCommand(UpdateWarrantyDto Dto) : IRequest<bool>;

public class UpdateWarrantyCommandHandler : IRequestHandler<UpdateWarrantyCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWarrantyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateWarrantyCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Warranty>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.WarrantyNumber = request.Dto.WarrantyNumber;
        item.ItemCovered = request.Dto.ItemCovered;
        item.WarrantyType = request.Dto.WarrantyType;
        item.StartDate = request.Dto.StartDate;
        item.EndDate = request.Dto.EndDate;
        item.Status = request.Dto.Status;
        item.Remarks = request.Dto.Remarks;
        item.PropertyUnitId = request.Dto.PropertyUnitId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
