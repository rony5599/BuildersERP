using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.TestReports;

public record GetAllTestReportsQuery(Guid? ProjectId = null) : IRequest<IReadOnlyList<TestReportDto>>;

public class GetAllTestReportsQueryHandler : IRequestHandler<GetAllTestReportsQuery, IReadOnlyList<TestReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllTestReportsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TestReportDto>> Handle(GetAllTestReportsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<TestReport>().Query()
            .Include(x => x.Project)
            .Include(x => x.Material)
            .AsQueryable();

        if (request.ProjectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);
        }

        var items = await query.OrderByDescending(x => x.TestDate).ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<TestReportDto>>(items);
    }
}
