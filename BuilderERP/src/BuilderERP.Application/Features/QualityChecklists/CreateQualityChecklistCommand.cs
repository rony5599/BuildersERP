using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.QualityChecklists;

public record CreateQualityChecklistCommand(CreateQualityChecklistDto Dto) : IRequest<Guid>;

public class CreateQualityChecklistCommandHandler : IRequestHandler<CreateQualityChecklistCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateQualityChecklistCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateQualityChecklistCommand request, CancellationToken cancellationToken)
    {
        var checklist = _mapper.Map<QualityChecklist>(request.Dto);
        await _unitOfWork.Repository<QualityChecklist>().AddAsync(checklist);
        await _unitOfWork.SaveChangesAsync();
        return checklist.Id;
    }
}
