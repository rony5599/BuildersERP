using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.MaintenanceRecords;

public record CreateMaintenanceRecordCommand(CreateMaintenanceRecordDto Dto) : IRequest<long>;

public class CreateMaintenanceRecordCommandHandler : IRequestHandler<CreateMaintenanceRecordCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateMaintenanceRecordCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateMaintenanceRecordCommand request, CancellationToken cancellationToken)
    {
        var record = _mapper.Map<MaintenanceRecord>(request.Dto);
        await _unitOfWork.Repository<MaintenanceRecord>().AddAsync(record);
        await _unitOfWork.SaveChangesAsync();
        return record.Id;
    }
}
