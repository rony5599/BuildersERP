using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Attendances;

public record CreateAttendanceCommand(CreateAttendanceDto Dto) : IRequest<long>;

public class CreateAttendanceCommandHandler : IRequestHandler<CreateAttendanceCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateAttendanceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateAttendanceCommand request, CancellationToken cancellationToken)
    {
        var attendance = _mapper.Map<Attendance>(request.Dto);
        await _unitOfWork.Repository<Attendance>().AddAsync(attendance);
        await _unitOfWork.SaveChangesAsync();

        return attendance.Id;
    }
}
