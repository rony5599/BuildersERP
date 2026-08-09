using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SitePhotos;

public record UpdateSitePhotoCommand(UpdateSitePhotoDto Dto) : IRequest<bool>;

public class UpdateSitePhotoCommandHandler : IRequestHandler<UpdateSitePhotoCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSitePhotoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSitePhotoCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SitePhoto>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.FilePath = request.Dto.FilePath;
        item.Caption = request.Dto.Caption;
        item.TakenDate = request.Dto.TakenDate;
        item.ProjectId = request.Dto.ProjectId;
        item.DailyProgressId = request.Dto.DailyProgressId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
