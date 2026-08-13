using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LandRegistrations;

public record CreateLandRegistrationCommand(CreateLandRegistrationDto Dto) : IRequest<Guid>;

public class CreateLandRegistrationCommandHandler : IRequestHandler<CreateLandRegistrationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateLandRegistrationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateLandRegistrationCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<LandRegistration>(request.Dto);
        var repository = _unitOfWork.Repository<LandRegistration>();

        await repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Id;
    }
}
