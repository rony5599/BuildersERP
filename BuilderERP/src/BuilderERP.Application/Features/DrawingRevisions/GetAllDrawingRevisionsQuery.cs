using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DrawingRevisions;

public record GetAllDrawingRevisionsQuery(Guid? DrawingId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<DrawingRevisionDto>>;

public class GetAllDrawingRevisionsQueryHandler : IRequestHandler<GetAllDrawingRevisionsQuery, PagedResult<DrawingRevisionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDrawingRevisionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<DrawingRevisionDto>> Handle(GetAllDrawingRevisionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<DrawingRevision>().Query()
            .Include(x => x.Drawing)
            .AsQueryable();

        if (request.DrawingId.HasValue)
        {
            query = query.Where(x => x.DrawingId == request.DrawingId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.RevisedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<DrawingRevisionDto>>(items);
        return new PagedResult<DrawingRevisionDto>(dtos, totalCount, page, pageSize);
    }
}
