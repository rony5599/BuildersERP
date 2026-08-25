using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SafetyTrainings;

public record GetAllSafetyTrainingsQuery(long? WorkerId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<SafetyTrainingDto>>;

public class GetAllSafetyTrainingsQueryHandler : IRequestHandler<GetAllSafetyTrainingsQuery, PagedResult<SafetyTrainingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSafetyTrainingsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<SafetyTrainingDto>> Handle(GetAllSafetyTrainingsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<SafetyTraining>().Query()
            .Include(x => x.Worker)
            .AsQueryable();

        if (request.WorkerId.HasValue)
        {
            query = query.Where(x => x.WorkerId == request.WorkerId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var trainings = await query
            .OrderByDescending(x => x.TrainingDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<SafetyTrainingDto>>(trainings);
        return new PagedResult<SafetyTrainingDto>(items, totalCount, page, pageSize);
    }
}
