using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DrawingRevisions;

public record GetAllDrawingRevisionsQuery(Guid? DrawingId = null) : IRequest<IReadOnlyList<DrawingRevisionDto>>;

public class GetAllDrawingRevisionsQueryHandler : IRequestHandler<GetAllDrawingRevisionsQuery, IReadOnlyList<DrawingRevisionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDrawingRevisionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DrawingRevisionDto>> Handle(GetAllDrawingRevisionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<DrawingRevision>().Query()
            .Include(x => x.Drawing)
            .AsQueryable();

        if (request.DrawingId.HasValue)
        {
            query = query.Where(x => x.DrawingId == request.DrawingId.Value);
        }

        var items = await query.OrderByDescending(x => x.RevisedDate).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DrawingRevisionDto>>(items);
    }
}
