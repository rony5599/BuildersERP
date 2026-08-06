using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.StockIssues;

public record GetAllStockIssuesQuery : IRequest<IReadOnlyList<StockIssueDto>>;

public class GetAllStockIssuesQueryHandler : IRequestHandler<GetAllStockIssuesQuery, IReadOnlyList<StockIssueDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllStockIssuesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<StockIssueDto>> Handle(GetAllStockIssuesQuery request, CancellationToken cancellationToken)
    {
        var issues = await _unitOfWork.Repository<StockIssue>().Query()
            .Include(i => i.Material)
            .Include(i => i.Warehouse)
            .ToListAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<StockIssueDto>>(issues);
    }
}
