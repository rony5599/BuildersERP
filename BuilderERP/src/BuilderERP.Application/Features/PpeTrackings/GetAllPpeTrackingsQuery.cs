using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PpeTrackings;

public record GetAllPpeTrackingsQuery(Guid? WorkerId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<PpeTrackingDto>>;

public class GetAllPpeTrackingsQueryHandler : IRequestHandler<GetAllPpeTrackingsQuery, PagedResult<PpeTrackingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPpeTrackingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<PpeTrackingDto>> Handle(GetAllPpeTrackingsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PpeTracking>().Query()
            .Include(x => x.Worker)
            .AsQueryable();

        if (request.WorkerId.HasValue)
        {
            query = query.Where(x => x.WorkerId == request.WorkerId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.IssueDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<IReadOnlyList<PpeTrackingDto>>(items);
        return new PagedResult<PpeTrackingDto>(mapped, totalCount, page, pageSize);
    }
}
