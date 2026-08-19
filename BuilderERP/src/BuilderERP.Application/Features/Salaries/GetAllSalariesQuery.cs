using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.Salaries;

public record GetAllSalariesQuery(Guid? WorkerId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<SalaryDto>>;

public class GetAllSalariesQueryHandler : IRequestHandler<GetAllSalariesQuery, PagedResult<SalaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllSalariesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<SalaryDto>> Handle(GetAllSalariesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Salary>().Query()
            .Include(x => x.Worker)
            .AsQueryable();

        if (request.WorkerId.HasValue)
        {
            query = query.Where(x => x.WorkerId == request.WorkerId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var salaries = await query
            .OrderByDescending(x => x.PeriodStart)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = _mapper.Map<IReadOnlyList<SalaryDto>>(salaries);
        return new PagedResult<SalaryDto>(items, totalCount, page, pageSize);
    }
}
