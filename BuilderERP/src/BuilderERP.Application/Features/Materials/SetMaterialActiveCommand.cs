using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Materials;

public record SetMaterialActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetMaterialActiveCommandHandler : IRequestHandler<SetMaterialActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetMaterialActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetMaterialActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Material>();
        var material = await repository.GetByIdAsync(request.Id);
        if (material is null)
        {
            return false;
        }

        material.IsActive = request.IsActive;
        repository.Update(material);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
