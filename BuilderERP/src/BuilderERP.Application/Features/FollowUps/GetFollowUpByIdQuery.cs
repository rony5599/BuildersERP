using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.FollowUps;

public record GetFollowUpByIdQuery(Guid Id) : IRequest<FollowUpDto?>;

public class GetFollowUpByIdQueryHandler : IRequestHandler<GetFollowUpByIdQuery, FollowUpDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetFollowUpByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<FollowUpDto?> Handle(GetFollowUpByIdQuery request, CancellationToken cancellationToken)
    {
        var followUp = await _unitOfWork.Repository<FollowUp>().GetByIdAsync(request.Id);
        return followUp is null ? null : _mapper.Map<FollowUpDto>(followUp);
    }
}
