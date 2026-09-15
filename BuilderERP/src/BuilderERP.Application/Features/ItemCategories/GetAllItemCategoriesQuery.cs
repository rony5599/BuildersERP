using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.ItemCategories;

public record GetAllItemCategoriesQuery(
    int Page = 1,
    int PageSize = 25,
    string? SearchTerm = null,
    long? ParentCategoryId = null,
    bool? IsActive = null) : IRequest<PagedResult<ItemCategoryDto>>;

public class GetAllItemCategoriesQueryHandler : IRequestHandler<GetAllItemCategoriesQuery, PagedResult<ItemCategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllItemCategoriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ItemCategoryDto>> Handle(GetAllItemCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<ItemCategory>().Query()
            .Include(c => c.ParentCategory)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(c => c.Code.Contains(term) || c.Name.Contains(term));
        }

        if (request.ParentCategoryId.HasValue)
        {
            query = query.Where(c => c.ParentCategoryId == request.ParentCategoryId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var categories = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<ItemCategoryDto>>(categories);
        return new PagedResult<ItemCategoryDto>(items, totalCount, page, pageSize);
    }
}
