using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PropertyUnits;

public record GetPropertyUnitByIdQuery(Guid Id) : IRequest<PropertyUnitDto?>;

public class GetPropertyUnitByIdQueryHandler : IRequestHandler<GetPropertyUnitByIdQuery, PropertyUnitDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPropertyUnitByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PropertyUnitDto?> Handle(GetPropertyUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var unit = await _unitOfWork.Repository<PropertyUnit>().GetByIdAsync(request.Id);
        return unit is null ? null : _mapper.Map<PropertyUnitDto>(unit);
    }
}
