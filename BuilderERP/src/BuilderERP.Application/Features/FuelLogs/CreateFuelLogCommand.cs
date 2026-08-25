using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.FuelLogs;

public record CreateFuelLogCommand(CreateFuelLogDto Dto) : IRequest<long>;

public class CreateFuelLogCommandHandler : IRequestHandler<CreateFuelLogCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateFuelLogCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateFuelLogCommand request, CancellationToken cancellationToken)
    {
        var log = _mapper.Map<FuelLog>(request.Dto);
        await _unitOfWork.Repository<FuelLog>().AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
        return log.Id;
    }
}
