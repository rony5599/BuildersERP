using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LandMutations;

public record CreateLandMutationCommand(CreateLandMutationDto Dto) : IRequest<Guid>;

public class CreateLandMutationCommandHandler : IRequestHandler<CreateLandMutationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateLandMutationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateLandMutationCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<LandMutation>(request.Dto);
        var repository = _unitOfWork.Repository<LandMutation>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
