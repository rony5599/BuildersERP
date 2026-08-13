using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.FlatHandovers;

public record UpdateFlatHandoverCommand(UpdateFlatHandoverDto Dto) : IRequest<bool>;

public class UpdateFlatHandoverCommandHandler : IRequestHandler<UpdateFlatHandoverCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFlatHandoverCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateFlatHandoverCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<FlatHandover>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.HandoverNumber = request.Dto.HandoverNumber;
        item.HandoverDate = request.Dto.HandoverDate;
        item.KeyIssuedTo = request.Dto.KeyIssuedTo;
        item.Status = request.Dto.Status;
        item.Remarks = request.Dto.Remarks;
        item.PropertyUnitId = request.Dto.PropertyUnitId;
        item.CustomerId = request.Dto.CustomerId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
