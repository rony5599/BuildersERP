using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ItemCategories;

public record SetItemCategoryActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetItemCategoryActiveCommandHandler : IRequestHandler<SetItemCategoryActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetItemCategoryActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetItemCategoryActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<ItemCategory>();
        var category = await repository.GetByIdAsync(request.Id);
        if (category is null)
        {
            return false;
        }

        category.IsActive = request.IsActive;
        repository.Update(category);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
