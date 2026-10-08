using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.BoqItems;

public record GetWorkGroupOptionsQuery : IRequest<IReadOnlyList<WorkGroupOptionDto>>;

public class GetWorkGroupOptionsQueryHandler : IRequestHandler<GetWorkGroupOptionsQuery, IReadOnlyList<WorkGroupOptionDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWorkGroupOptionsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<WorkGroupOptionDto>> Handle(GetWorkGroupOptionsQuery request, CancellationToken cancellationToken)
    {
        var groups = await _unitOfWork.Repository<WorkGroup>().Query()
            .AsNoTracking()
            .OrderBy(x => x.GroupCode)
            .Select(x => new { x.Id, x.ParentGroupId, x.GroupCode, x.GroupName })
            .ToListAsync(cancellationToken);

        var depths = new Dictionary<long, int>();
        int GetDepth(long id, HashSet<long>? path = null)
        {
            if (depths.TryGetValue(id, out var known)) return known;
            path ??= new HashSet<long>();
            if (!path.Add(id)) return 0;
            var group = groups.First(x => x.Id == id);
            var depth = group.ParentGroupId is long parentId && groups.Any(x => x.Id == parentId)
                ? GetDepth(parentId, path) + 1
                : 0;
            depths[id] = depth;
            return depth;
        }

        return groups.Select(x => new WorkGroupOptionDto
        {
            Id = x.Id,
            ParentGroupId = x.ParentGroupId,
            GroupCode = x.GroupCode,
            GroupName = x.GroupName,
            Depth = GetDepth(x.Id)
        }).ToList();
    }
}
