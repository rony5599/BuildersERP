using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.PunchLists;

public record GetAllPunchListsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<PunchListDto>>;

public class GetAllPunchListsQueryHandler : IRequestHandler<GetAllPunchListsQuery, IReadOnlyList<PunchListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllPunchListsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PunchListDto>> Handle(GetAllPunchListsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PunchList>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var items = await query
            .OrderBy(x => x.DueDate == null)
            .ThenBy(x => x.DueDate)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PunchListDto>>(items);
    }
}
