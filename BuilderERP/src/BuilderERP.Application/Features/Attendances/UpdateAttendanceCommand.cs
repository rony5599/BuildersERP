using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Attendances;

public record UpdateAttendanceCommand(UpdateAttendanceDto Dto) : IRequest<bool>;

public class UpdateAttendanceCommandHandler : IRequestHandler<UpdateAttendanceCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAttendanceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Attendance>();
        var attendance = await repository.GetByIdAsync(request.Dto.Id);
        if (attendance is null)
        {
            return false;
        }

        attendance.WorkerId = request.Dto.WorkerId;
        attendance.ProjectId = request.Dto.ProjectId;
        attendance.AttendanceDate = request.Dto.AttendanceDate;
        attendance.Status = request.Dto.Status;
        attendance.HoursWorked = request.Dto.HoursWorked;

        repository.Update(attendance);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
