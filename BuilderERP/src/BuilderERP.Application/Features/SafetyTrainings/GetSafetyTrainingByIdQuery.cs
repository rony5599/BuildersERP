using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SafetyTrainings;

public record GetSafetyTrainingByIdQuery(Guid Id) : IRequest<SafetyTrainingDto?>;

public class GetSafetyTrainingByIdQueryHandler : IRequestHandler<GetSafetyTrainingByIdQuery, SafetyTrainingDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSafetyTrainingByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SafetyTrainingDto?> Handle(GetSafetyTrainingByIdQuery request, CancellationToken cancellationToken)
    {
        var training = await _unitOfWork.Repository<SafetyTraining>().GetByIdAsync(request.Id);
        return training is null ? null : _mapper.Map<SafetyTrainingDto>(training);
    }
}
