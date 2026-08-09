using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Drawings;

public record GetAllDrawingsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<DrawingDto>>;

public class GetAllDrawingsQueryHandler : IRequestHandler<GetAllDrawingsQuery, IReadOnlyList<DrawingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDrawingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DrawingDto>> Handle(GetAllDrawingsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Drawing>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var items = await query.OrderBy(x => x.DrawingNumber).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DrawingDto>>(items);
    }
}
