using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Materials;

public record GetMaterialByIdQuery(Guid Id) : IRequest<MaterialDto?>;

public class GetMaterialByIdQueryHandler : IRequestHandler<GetMaterialByIdQuery, MaterialDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMaterialByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MaterialDto?> Handle(GetMaterialByIdQuery request, CancellationToken cancellationToken)
    {
        var material = await _unitOfWork.Repository<Material>().GetByIdAsync(request.Id);
        return material is null ? null : _mapper.Map<MaterialDto>(material);
    }
}
