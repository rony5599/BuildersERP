using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.FuelLogs;

public record GetFuelLogByIdQuery(long Id) : IRequest<FuelLogDto?>;

public class GetFuelLogByIdQueryHandler : IRequestHandler<GetFuelLogByIdQuery, FuelLogDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetFuelLogByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<FuelLogDto?> Handle(GetFuelLogByIdQuery request, CancellationToken cancellationToken)
    {
        var log = await _unitOfWork.Repository<FuelLog>().GetByIdAsync(request.Id);
        return log is null ? null : _mapper.Map<FuelLogDto>(log);
    }
}
