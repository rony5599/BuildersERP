using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Materials;

public record GetAllMaterialsQuery : IRequest<IReadOnlyList<MaterialDto>>;

public class GetAllMaterialsQueryHandler : IRequestHandler<GetAllMaterialsQuery, IReadOnlyList<MaterialDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMaterialsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<MaterialDto>> Handle(GetAllMaterialsQuery request, CancellationToken cancellationToken)
    {
        var materials = await _unitOfWork.Repository<Material>().GetAllAsync();
        return _mapper.Map<IReadOnlyList<MaterialDto>>(materials);
    }
}
