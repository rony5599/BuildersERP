using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Milestones;

public record CreateMilestoneCommand(CreateMilestoneDto Dto) : IRequest<Guid>;

public class CreateMilestoneCommandHandler : IRequestHandler<CreateMilestoneCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateMilestoneCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateMilestoneCommand request, CancellationToken cancellationToken)
    {
        var milestone = _mapper.Map<Milestone>(request.Dto);
        await _unitOfWork.Repository<Milestone>().AddAsync(milestone);
        await _unitOfWork.SaveChangesAsync();
        return milestone.Id;
    }
}
