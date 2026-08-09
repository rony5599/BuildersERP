using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Workers;

public record UpdateWorkerCommand(UpdateWorkerDto Dto) : IRequest<bool>;

public class UpdateWorkerCommandHandler : IRequestHandler<UpdateWorkerCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWorkerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateWorkerCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Worker>();
        var worker = await repository.GetByIdAsync(request.Dto.Id);
        if (worker is null)
        {
            return false;
        }

        worker.WorkerCode = request.Dto.WorkerCode;
        worker.Name = request.Dto.Name;
        worker.Phone = request.Dto.Phone;
        worker.Address = request.Dto.Address;
        worker.Trade = request.Dto.Trade;
        worker.DailyWageRate = request.Dto.DailyWageRate;
        worker.JoinDate = request.Dto.JoinDate;
        worker.ContractorId = request.Dto.ContractorId;

        repository.Update(worker);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
