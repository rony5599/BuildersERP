using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.CollectionTargets;

public record GetCollectionTargetByIdQuery(Guid Id) : IRequest<CollectionTargetDto?>;

public class GetCollectionTargetByIdQueryHandler : IRequestHandler<GetCollectionTargetByIdQuery, CollectionTargetDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCollectionTargetByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CollectionTargetDto?> Handle(GetCollectionTargetByIdQuery request, CancellationToken cancellationToken)
    {
        var target = await _unitOfWork.Repository<CollectionTarget>().GetByIdAsync(request.Id);
        return target is null ? null : _mapper.Map<CollectionTargetDto>(target);
    }
}
