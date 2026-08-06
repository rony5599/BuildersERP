using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.FollowUps;

public record CreateFollowUpCommand(CreateFollowUpDto Dto) : IRequest<Guid>;

public class CreateFollowUpCommandHandler : IRequestHandler<CreateFollowUpCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateFollowUpCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateFollowUpCommand request, CancellationToken cancellationToken)
    {
        var followUp = _mapper.Map<FollowUp>(request.Dto);
        await _unitOfWork.Repository<FollowUp>().AddAsync(followUp);
        await _unitOfWork.SaveChangesAsync();

        return followUp.Id;
    }
}
