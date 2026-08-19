using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Warranties;

public record GetAllWarrantiesQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<WarrantyDto>>;

public class GetAllWarrantiesQueryHandler : IRequestHandler<GetAllWarrantiesQuery, PagedResult<WarrantyDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllWarrantiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<WarrantyDto>> Handle(GetAllWarrantiesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Warranty>().Query()
            .Include(x => x.PropertyUnit)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.WarrantyNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<IReadOnlyList<WarrantyDto>>(items);
        return new PagedResult<WarrantyDto>(mapped, totalCount, page, pageSize);
    }
}
