using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DelayEvents;

public record GetDelayEventByIdQuery(Guid Id) : IRequest<DelayEventDto?>;

public class GetDelayEventByIdQueryHandler : IRequestHandler<GetDelayEventByIdQuery, DelayEventDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDelayEventByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DelayEventDto?> Handle(GetDelayEventByIdQuery request, CancellationToken cancellationToken)
    {
        var delayEvent = await _unitOfWork.Repository<DelayEvent>().Query()
            .Include(x => x.Project)
            .Include(x => x.WbsTask)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return delayEvent is null ? null : _mapper.Map<DelayEventDto>(delayEvent);
    }
}
