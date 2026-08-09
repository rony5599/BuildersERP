using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PpeTrackings;

public record GetAllPpeTrackingsQuery(Guid? WorkerId = null) : IRequest<IReadOnlyList<PpeTrackingDto>>;

public class GetAllPpeTrackingsQueryHandler : IRequestHandler<GetAllPpeTrackingsQuery, IReadOnlyList<PpeTrackingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPpeTrackingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PpeTrackingDto>> Handle(GetAllPpeTrackingsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PpeTracking>().Query()
            .Include(x => x.Worker)
            .AsQueryable();

        if (request.WorkerId.HasValue)
        {
            query = query.Where(x => x.WorkerId == request.WorkerId.Value);
        }

        var items = await query
            .OrderByDescending(x => x.IssueDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IReadOnlyList<PpeTrackingDto>>(items);
    }
}
