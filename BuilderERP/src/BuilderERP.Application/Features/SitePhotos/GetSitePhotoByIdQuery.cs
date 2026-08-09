using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SitePhotos;

public record GetSitePhotoByIdQuery(Guid Id) : IRequest<SitePhotoDto?>;

public class GetSitePhotoByIdQueryHandler : IRequestHandler<GetSitePhotoByIdQuery, SitePhotoDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSitePhotoByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SitePhotoDto?> Handle(GetSitePhotoByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<SitePhoto>().Query()
            .Include(x => x.Project)
            .Include(x => x.DailyProgress)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<SitePhotoDto>(item);
    }
}
