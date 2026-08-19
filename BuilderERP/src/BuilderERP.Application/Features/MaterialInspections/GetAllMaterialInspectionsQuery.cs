using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.MaterialInspections;

public record GetAllMaterialInspectionsQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<MaterialInspectionDto>>;

public class GetAllMaterialInspectionsQueryHandler : IRequestHandler<GetAllMaterialInspectionsQuery, PagedResult<MaterialInspectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMaterialInspectionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<MaterialInspectionDto>> Handle(GetAllMaterialInspectionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<MaterialInspection>().Query()
            .Include(x => x.Project)
            .Include(x => x.Material)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var inspections = await query
            .OrderByDescending(x => x.InspectionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<MaterialInspectionDto>>(inspections);
        return new PagedResult<MaterialInspectionDto>(items, totalCount, page, pageSize);
    }
}
