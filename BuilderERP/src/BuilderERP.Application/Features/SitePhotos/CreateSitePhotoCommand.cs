using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SitePhotos;

public record CreateSitePhotoCommand(CreateSitePhotoDto Dto) : IRequest<Guid>;

public class CreateSitePhotoCommandHandler : IRequestHandler<CreateSitePhotoCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSitePhotoCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateSitePhotoCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<SitePhoto>(request.Dto);
        await _unitOfWork.Repository<SitePhoto>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return item.Id;
    }
}
