using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Overtimes;

public record CreateOvertimeCommand(CreateOvertimeDto Dto) : IRequest<Guid>;

public class CreateOvertimeCommandHandler : IRequestHandler<CreateOvertimeCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateOvertimeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateOvertimeCommand request, CancellationToken cancellationToken)
    {
        var overtime = _mapper.Map<Overtime>(request.Dto);
        overtime.Amount = overtime.Hours * overtime.RatePerHour;
        await _unitOfWork.Repository<Overtime>().AddAsync(overtime);
        await _unitOfWork.SaveChangesAsync();
        return overtime.Id;
    }
}
