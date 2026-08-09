using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Overtimes;

public record UpdateOvertimeCommand(UpdateOvertimeDto Dto) : IRequest<bool>;

public class UpdateOvertimeCommandHandler : IRequestHandler<UpdateOvertimeCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOvertimeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateOvertimeCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Overtime>();
        var overtime = await repository.GetByIdAsync(request.Dto.Id);
        if (overtime is null)
        {
            return false;
        }

        overtime.OvertimeDate = request.Dto.OvertimeDate;
        overtime.Hours = request.Dto.Hours;
        overtime.RatePerHour = request.Dto.RatePerHour;
        overtime.WorkerId = request.Dto.WorkerId;
        overtime.ProjectId = request.Dto.ProjectId;
        overtime.Amount = overtime.Hours * overtime.RatePerHour;

        repository.Update(overtime);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
