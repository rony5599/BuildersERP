using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SitePhotos;

public record GetAllSitePhotosQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<SitePhotoDto>>;

public class GetAllSitePhotosQueryHandler : IRequestHandler<GetAllSitePhotosQuery, IReadOnlyList<SitePhotoDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSitePhotosQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SitePhotoDto>> Handle(GetAllSitePhotosQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SitePhoto>().Query()
            .Include(x => x.Project)
            .Include(x => x.DailyProgress)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var items = await query.OrderByDescending(x => x.TakenDate).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SitePhotoDto>>(items);
    }
}
