using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Materials;

public record CreateMaterialCommand(CreateMaterialDto Dto) : IRequest<long>;

public class CreateMaterialCommandHandler : IRequestHandler<CreateMaterialCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateMaterialCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateMaterialCommand request, CancellationToken cancellationToken)
    {
        var material = _mapper.Map<Material>(request.Dto);
        await _unitOfWork.Repository<Material>().AddAsync(material);
        await _unitOfWork.SaveChangesAsync();
        return material.Id;
    }
}
