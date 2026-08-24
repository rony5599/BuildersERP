using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ItemCategories;

public record UpdateItemCategoryCommand(UpdateItemCategoryDto Dto) : IRequest<bool>;

public class UpdateItemCategoryCommandHandler : IRequestHandler<UpdateItemCategoryCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateItemCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateItemCategoryCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<ItemCategory>();
        var category = await repository.GetByIdAsync(request.Dto.Id);
        if (category is null)
        {
            return false;
        }

        category.Code = request.Dto.Code;
        category.Name = request.Dto.Name;
        category.ParentCategoryId = request.Dto.ParentCategoryId;

        repository.Update(category);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
