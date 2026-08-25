using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SafetyTrainings;

public record CreateSafetyTrainingCommand(CreateSafetyTrainingDto Dto) : IRequest<long>;

public class CreateSafetyTrainingCommandHandler : IRequestHandler<CreateSafetyTrainingCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSafetyTrainingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateSafetyTrainingCommand request, CancellationToken cancellationToken)
    {
        var training = _mapper.Map<SafetyTraining>(request.Dto);
        await _unitOfWork.Repository<SafetyTraining>().AddAsync(training);
        await _unitOfWork.SaveChangesAsync();

        return training.Id;
    }
}
