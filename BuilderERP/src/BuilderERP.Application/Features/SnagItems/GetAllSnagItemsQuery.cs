using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SnagItems;

public record GetAllSnagItemsQuery(int Page = 1, int PageSize = 25) : IRequest<PagedResult<SnagItemDto>>;

public class GetAllSnagItemsQueryHandler : IRequestHandler<GetAllSnagItemsQuery, PagedResult<SnagItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSnagItemsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<SnagItemDto>> Handle(GetAllSnagItemsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SnagItem>().Query()
            .Include(x => x.PropertyUnit)
            .AsQueryable();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderBy(x => x.SnagNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<SnagItemDto>>(results);
        return new PagedResult<SnagItemDto>(items, totalCount, page, pageSize);
    }
}
