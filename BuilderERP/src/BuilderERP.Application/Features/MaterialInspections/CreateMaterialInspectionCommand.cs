using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.MaterialInspections;

public record CreateMaterialInspectionCommand(CreateMaterialInspectionDto Dto) : IRequest<long>;

public class CreateMaterialInspectionCommandHandler : IRequestHandler<CreateMaterialInspectionCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateMaterialInspectionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateMaterialInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = _mapper.Map<MaterialInspection>(request.Dto);
        await _unitOfWork.Repository<MaterialInspection>().AddAsync(inspection);
        await _unitOfWork.SaveChangesAsync();
        return inspection.Id;
    }
}
