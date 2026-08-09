using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.DailyProgresses;

public record GetAllDailyProgressesQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<DailyProgressDto>>;

public class GetAllDailyProgressesQueryHandler : IRequestHandler<GetAllDailyProgressesQuery, IReadOnlyList<DailyProgressDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllDailyProgressesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DailyProgressDto>> Handle(GetAllDailyProgressesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<DailyProgress>().Query().Include(x => x.Project).AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var items = await query.OrderByDescending(x => x.ProgressDate).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DailyProgressDto>>(items);
    }
}
