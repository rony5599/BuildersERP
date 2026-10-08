using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.BoqItems;

public record GetAllBoqItemsQuery(long? ProjectId = null, string? BoqName = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<BoqItemDto>>;

public class GetAllBoqItemsQueryHandler : IRequestHandler<GetAllBoqItemsQuery, PagedResult<BoqItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllBoqItemsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<BoqItemDto>> Handle(GetAllBoqItemsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<BoqItem>().Query()
            .Include(x => x.Boq).ThenInclude(x => x.Project)
            .Include(x => x.WorkGroup).AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.Boq.ProjectId == request.ProjectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.BoqName))
        {
            var boqName = request.BoqName.Trim();
            query = query.Where(x => x.Boq.BoqName.Contains(boqName));
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Boq.BoqName).ThenBy(x => x.WorkGroup.GroupCode).ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<BoqItemDto>>(items);
        return new PagedResult<BoqItemDto>(dtos, totalCount, page, pageSize);
    }
}
