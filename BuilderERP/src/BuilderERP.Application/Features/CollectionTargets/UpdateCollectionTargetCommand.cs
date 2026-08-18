using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CollectionTargets;

public record UpdateCollectionTargetCommand(UpdateCollectionTargetDto Dto) : IRequest<bool>;

public class UpdateCollectionTargetCommandHandler : IRequestHandler<UpdateCollectionTargetCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCollectionTargetCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCollectionTargetCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<CollectionTarget>();
        var target = await repository.GetByIdAsync(request.Dto.Id);
        if (target is null)
        {
            return false;
        }

        target.Year = request.Dto.Year;
        target.Month = request.Dto.Month;
        target.TargetAmount = request.Dto.TargetAmount;
        target.CollectionOfficerId = request.Dto.CollectionOfficerId;

        repository.Update(target);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
