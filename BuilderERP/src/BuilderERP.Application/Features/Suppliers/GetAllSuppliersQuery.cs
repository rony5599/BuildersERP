using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Suppliers;

public record GetAllSuppliersQuery(
    int Page = 1,
    int PageSize = 25,
    string? SearchTerm = null,
    string? VendorCategory = null,
    bool? IsActive = null) : IRequest<PagedResult<SupplierDto>>;

public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, PagedResult<SupplierDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSuppliersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<SupplierDto>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Supplier>().Query().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            query = query.Where(s =>
                s.SupplierCode.Contains(term) ||
                s.Name.Contains(term) ||
                s.Phone.Contains(term) ||
                (s.Email != null && s.Email.Contains(term)) ||
                (s.ContactPerson != null && s.ContactPerson.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(request.VendorCategory))
        {
            var term = request.VendorCategory.Trim();
            query = query.Where(s => s.VendorCategory != null && s.VendorCategory.Contains(term));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == request.IsActive.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var suppliers = await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<SupplierDto>>(suppliers);
        return new PagedResult<SupplierDto>(items, totalCount, page, pageSize);
    }
}
