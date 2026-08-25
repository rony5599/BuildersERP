using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Workers;

public record GetAllWorkersQuery(long? ContractorId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<WorkerDto>>;

public class GetAllWorkersQueryHandler : IRequestHandler<GetAllWorkersQuery, PagedResult<WorkerDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllWorkersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<WorkerDto>> Handle(GetAllWorkersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Worker>().Query()
            .Include(x => x.Contractor)
            .AsQueryable();

        if (request.ContractorId.HasValue)
        {
            query = query.Where(x => x.ContractorId == request.ContractorId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var workers = await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<WorkerDto>>(workers);
        return new PagedResult<WorkerDto>(items, totalCount, page, pageSize);
    }
}
