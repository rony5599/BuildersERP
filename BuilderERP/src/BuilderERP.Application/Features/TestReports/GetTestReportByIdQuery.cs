using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.TestReports;

public record GetTestReportByIdQuery(long Id) : IRequest<TestReportDto?>;

public class GetTestReportByIdQueryHandler : IRequestHandler<GetTestReportByIdQuery, TestReportDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTestReportByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TestReportDto?> Handle(GetTestReportByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<TestReport>().Query()
            .Include(x => x.Project)
            .Include(x => x.Material)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return item is null ? null : _mapper.Map<TestReportDto>(item);
    }
}
