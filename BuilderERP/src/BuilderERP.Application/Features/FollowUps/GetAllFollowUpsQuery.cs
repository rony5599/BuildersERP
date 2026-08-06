using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.FollowUps;

public record GetAllFollowUpsQuery : IRequest<IReadOnlyList<FollowUpDto>>;

public class GetAllFollowUpsQueryHandler : IRequestHandler<GetAllFollowUpsQuery, IReadOnlyList<FollowUpDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllFollowUpsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<FollowUpDto>> Handle(GetAllFollowUpsQuery request, CancellationToken cancellationToken)
    {
        var followUps = await _unitOfWork.Repository<FollowUp>().Query()
            .Include(f => f.Lead)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<FollowUpDto>>(followUps);
    }
}
