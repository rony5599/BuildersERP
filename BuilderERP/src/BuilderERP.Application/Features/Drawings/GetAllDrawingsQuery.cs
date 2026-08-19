using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Drawings;

public record GetAllDrawingsQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<DrawingDto>>;

public class GetAllDrawingsQueryHandler : IRequestHandler<GetAllDrawingsQuery, PagedResult<DrawingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDrawingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<DrawingDto>> Handle(GetAllDrawingsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Drawing>().Query()
            .Include(x => x.Project)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.DrawingNumber)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<DrawingDto>>(items);
        return new PagedResult<DrawingDto>(dtos, totalCount, page, pageSize);
    }
}
