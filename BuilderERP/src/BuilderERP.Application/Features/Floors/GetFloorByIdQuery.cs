using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Floors;

public record GetFloorByIdQuery(long Id) : IRequest<FloorDto?>;

public class GetFloorByIdQueryHandler : IRequestHandler<GetFloorByIdQuery, FloorDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetFloorByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<FloorDto?> Handle(GetFloorByIdQuery request, CancellationToken cancellationToken)
    {
        var floor = await _unitOfWork.Repository<Floor>().GetByIdAsync(request.Id);
        return floor is null ? null : _mapper.Map<FloorDto>(floor);
    }
}
