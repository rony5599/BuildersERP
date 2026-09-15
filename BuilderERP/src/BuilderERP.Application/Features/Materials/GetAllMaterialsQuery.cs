using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Materials;

public record GetAllMaterialsQuery(
    long? ProjectId = null,
    int Page = 1,
    int PageSize = 25,
    string? SearchTerm = null,
    long? CategoryId = null,
    bool? IsActive = null) : IRequest<PagedResult<MaterialDto>>;

public class GetAllMaterialsQueryHandler : IRequestHandler<GetAllMaterialsQuery, PagedResult<MaterialDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllMaterialsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<MaterialDto>> Handle(GetAllMaterialsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Material>().Query().Include(m => m.Category).AsQueryable();

        if (request.ProjectId.HasValue)
        {
            var materialIdsInProject = _unitOfWork.Repository<Stock>().Query()
                .Where(s => s.Warehouse.ProjectId == request.ProjectId.Value)
                .Select(s => s.MaterialId)
                .Distinct();

            query = query.Where(m => materialIdsInProject.Contains(m.Id));
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(m =>
                m.MaterialCode.Contains(term) ||
                m.Name.Contains(term) ||
                (m.Barcode != null && m.Barcode.Contains(term)));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(m => m.CategoryId == request.CategoryId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(m => m.IsActive == request.IsActive.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var materials = await query
            .OrderBy(m => m.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<MaterialDto>>(materials);
        return new PagedResult<MaterialDto>(items, totalCount, page, pageSize);
    }
}
