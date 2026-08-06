using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Floors;

public record GetAllFloorsQuery : IRequest<IReadOnlyList<FloorDto>>;

public class GetAllFloorsQueryHandler : IRequestHandler<GetAllFloorsQuery, IReadOnlyList<FloorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllFloorsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<FloorDto>> Handle(GetAllFloorsQuery request, CancellationToken cancellationToken)
    {
        var floors = await _unitOfWork.Repository<Floor>().Query().Include(f => f.Tower).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<FloorDto>>(floors);
    }
}
