using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SafetyTrainings;

public record GetAllSafetyTrainingsQuery(Guid? WorkerId = null) : IRequest<IReadOnlyList<SafetyTrainingDto>>;

public class GetAllSafetyTrainingsQueryHandler : IRequestHandler<GetAllSafetyTrainingsQuery, IReadOnlyList<SafetyTrainingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSafetyTrainingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SafetyTrainingDto>> Handle(GetAllSafetyTrainingsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SafetyTraining>().Query()
            .Include(x => x.Worker)
            .AsQueryable();

        if (request.WorkerId.HasValue)
        {
            query = query.Where(x => x.WorkerId == request.WorkerId.Value);
        }

        var trainings = await query
            .OrderByDescending(x => x.TrainingDate)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SafetyTrainingDto>>(trainings);
    }
}
