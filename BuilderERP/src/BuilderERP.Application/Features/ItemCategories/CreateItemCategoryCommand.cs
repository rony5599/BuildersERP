using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.ItemCategories;

public record CreateItemCategoryCommand(CreateItemCategoryDto Dto) : IRequest<long>;

public class CreateItemCategoryCommandHandler : IRequestHandler<CreateItemCategoryCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateItemCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateItemCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = _mapper.Map<ItemCategory>(request.Dto);
        await _unitOfWork.Repository<ItemCategory>().AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return category.Id;
    }
}
