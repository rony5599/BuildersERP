using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SnagItems;

public record CreateSnagItemCommand(CreateSnagItemDto Dto) : IRequest<Guid>;

public class CreateSnagItemCommandHandler : IRequestHandler<CreateSnagItemCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSnagItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateSnagItemCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<SnagItem>(request.Dto);
        var repository = _unitOfWork.Repository<SnagItem>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
