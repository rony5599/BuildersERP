using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SafetyInspections;

public record CreateSafetyInspectionCommand(CreateSafetyInspectionDto Dto) : IRequest<long>;

public class CreateSafetyInspectionCommandHandler : IRequestHandler<CreateSafetyInspectionCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSafetyInspectionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateSafetyInspectionCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<SafetyInspection>(request.Dto);
        await _unitOfWork.Repository<SafetyInspection>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}
