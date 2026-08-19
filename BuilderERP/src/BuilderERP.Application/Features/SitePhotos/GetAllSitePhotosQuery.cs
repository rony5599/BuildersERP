using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SitePhotos;

public record GetAllSitePhotosQuery(Guid? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<SitePhotoDto>>;

public class GetAllSitePhotosQueryHandler : IRequestHandler<GetAllSitePhotosQuery, PagedResult<SitePhotoDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSitePhotosQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<SitePhotoDto>> Handle(GetAllSitePhotosQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SitePhoto>().Query()
            .Include(x => x.Project)
            .Include(x => x.DailyProgress)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.TakenDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<SitePhotoDto>>(items);
        return new PagedResult<SitePhotoDto>(dtos, totalCount, page, pageSize);
    }
}
