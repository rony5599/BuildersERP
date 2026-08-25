using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SafetyInspections;

public record GetSafetyInspectionByIdQuery(long Id) : IRequest<SafetyInspectionDto?>;

public class GetSafetyInspectionByIdQueryHandler : IRequestHandler<GetSafetyInspectionByIdQuery, SafetyInspectionDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSafetyInspectionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SafetyInspectionDto?> Handle(GetSafetyInspectionByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<SafetyInspection>().GetByIdAsync(request.Id);
        return item is null ? null : _mapper.Map<SafetyInspectionDto>(item);
    }
}
