using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.MaterialInspections;

public record GetMaterialInspectionByIdQuery(Guid Id) : IRequest<MaterialInspectionDto?>;

public class GetMaterialInspectionByIdQueryHandler : IRequestHandler<GetMaterialInspectionByIdQuery, MaterialInspectionDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMaterialInspectionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<MaterialInspectionDto?> Handle(GetMaterialInspectionByIdQuery request, CancellationToken cancellationToken)
    {
        var inspection = await _unitOfWork.Repository<MaterialInspection>().GetByIdAsync(request.Id);
        return inspection is null ? null : _mapper.Map<MaterialInspectionDto>(inspection);
    }
}
