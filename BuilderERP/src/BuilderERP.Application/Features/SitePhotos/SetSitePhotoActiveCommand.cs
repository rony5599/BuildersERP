using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SitePhotos;

public record SetSitePhotoActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetSitePhotoActiveCommandHandler : IRequestHandler<SetSitePhotoActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetSitePhotoActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetSitePhotoActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SitePhoto>();
        var item = await repository.GetByIdAsync(request.Id);
        if (item is null)
        {
            return false;
        }

        item.IsActive = request.IsActive;
        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
