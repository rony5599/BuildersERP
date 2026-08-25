using AutoMapper;
using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.TestReports;

public record GetAllTestReportsQuery(long? ProjectId = null, int Page = 1, int PageSize = 25) : IRequest<PagedResult<TestReportDto>>;

public class GetAllTestReportsQueryHandler : IRequestHandler<GetAllTestReportsQuery, PagedResult<TestReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllTestReportsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<TestReportDto>> Handle(GetAllTestReportsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<TestReport>().Query()
            .Include(x => x.Project)
            .Include(x => x.Material)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 25 : request.PageSize;

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.TestDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<IReadOnlyList<TestReportDto>>(items);
        return new PagedResult<TestReportDto>(mapped, totalCount, page, pageSize);
    }
}
