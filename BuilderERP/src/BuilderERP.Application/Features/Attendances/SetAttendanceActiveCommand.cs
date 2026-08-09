using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Attendances;

public record SetAttendanceActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetAttendanceActiveCommandHandler : IRequestHandler<SetAttendanceActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetAttendanceActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetAttendanceActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Attendance>();
        var attendance = await repository.GetByIdAsync(request.Id);
        if (attendance is null)
        {
            return false;
        }

        attendance.IsActive = request.IsActive;
        repository.Update(attendance);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
