using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CollectionTargets;

public record CreateCollectionTargetCommand(CreateCollectionTargetDto Dto) : IRequest<Guid>;

public class CreateCollectionTargetCommandHandler : IRequestHandler<CreateCollectionTargetCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCollectionTargetCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateCollectionTargetCommand request, CancellationToken cancellationToken)
    {
        var target = _mapper.Map<CollectionTarget>(request.Dto);
        await _unitOfWork.Repository<CollectionTarget>().AddAsync(target);
        await _unitOfWork.SaveChangesAsync();

        return target.Id;
    }
}
