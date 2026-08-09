using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.BoqItems;

public record GetAllBoqItemsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<BoqItemDto>>;

public class GetAllBoqItemsQueryHandler : IRequestHandler<GetAllBoqItemsQuery, IReadOnlyList<BoqItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBoqItemsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BoqItemDto>> Handle(GetAllBoqItemsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<BoqItem>().Query().Include(x => x.Project).AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var items = await query.OrderBy(x => x.ItemCode).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<BoqItemDto>>(items);
    }
}
