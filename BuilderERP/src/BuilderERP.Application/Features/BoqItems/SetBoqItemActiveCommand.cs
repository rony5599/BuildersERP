using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.BoqItems;

public record SetBoqItemActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetBoqItemActiveCommandHandler : IRequestHandler<SetBoqItemActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetBoqItemActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetBoqItemActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<BoqHeader>();
        var boq = await repository.GetByIdAsync(request.Id);
        if (boq is null)
        {
            return false;
        }

        boq.IsActive = request.IsActive;
        repository.Update(boq);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
