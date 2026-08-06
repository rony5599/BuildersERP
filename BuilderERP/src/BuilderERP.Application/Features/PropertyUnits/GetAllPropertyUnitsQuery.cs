using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PropertyUnits;

public record GetAllPropertyUnitsQuery : IRequest<IReadOnlyList<PropertyUnitDto>>;

public class GetAllPropertyUnitsQueryHandler : IRequestHandler<GetAllPropertyUnitsQuery, IReadOnlyList<PropertyUnitDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPropertyUnitsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PropertyUnitDto>> Handle(GetAllPropertyUnitsQuery request, CancellationToken cancellationToken)
    {
        var units = await _unitOfWork.Repository<PropertyUnit>().Query().Include(u => u.Floor).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PropertyUnitDto>>(units);
    }
}
