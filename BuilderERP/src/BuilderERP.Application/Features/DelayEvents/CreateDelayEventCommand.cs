using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DelayEvents;

public record CreateDelayEventCommand(CreateDelayEventDto Dto) : IRequest<long>;

public class CreateDelayEventCommandHandler : IRequestHandler<CreateDelayEventCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateDelayEventCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateDelayEventCommand request, CancellationToken cancellationToken)
    {
        var delayEvent = _mapper.Map<DelayEvent>(request.Dto);
        await _unitOfWork.Repository<DelayEvent>().AddAsync(delayEvent);
        await _unitOfWork.SaveChangesAsync();
        return delayEvent.Id;
    }
}
