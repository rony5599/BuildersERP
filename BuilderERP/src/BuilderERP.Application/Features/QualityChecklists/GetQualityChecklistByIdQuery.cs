using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.QualityChecklists;

public record GetQualityChecklistByIdQuery(Guid Id) : IRequest<QualityChecklistDto?>;

public class GetQualityChecklistByIdQueryHandler : IRequestHandler<GetQualityChecklistByIdQuery, QualityChecklistDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetQualityChecklistByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<QualityChecklistDto?> Handle(GetQualityChecklistByIdQuery request, CancellationToken cancellationToken)
    {
        var checklist = await _unitOfWork.Repository<QualityChecklist>().GetByIdAsync(request.Id);
        return checklist is null ? null : _mapper.Map<QualityChecklistDto>(checklist);
    }
}
