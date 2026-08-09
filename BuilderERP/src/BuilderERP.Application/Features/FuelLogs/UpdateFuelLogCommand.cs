using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.FuelLogs;

public record UpdateFuelLogCommand(UpdateFuelLogDto Dto) : IRequest<bool>;

public class UpdateFuelLogCommandHandler : IRequestHandler<UpdateFuelLogCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFuelLogCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateFuelLogCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<FuelLog>();
        var log = await repository.GetByIdAsync(request.Dto.Id);
        if (log is null)
        {
            return false;
        }

        log.EquipmentId = request.Dto.EquipmentId;
        log.LogDate = request.Dto.LogDate;
        log.FuelQuantity = request.Dto.FuelQuantity;
        log.FuelCost = request.Dto.FuelCost;
        log.MeterReading = request.Dto.MeterReading;
        log.Remarks = request.Dto.Remarks;

        repository.Update(log);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
