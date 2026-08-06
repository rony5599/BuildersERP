using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockIssues;

public record GetStockIssueByIdQuery(Guid Id) : IRequest<StockIssueDto?>;

public class GetStockIssueByIdQueryHandler : IRequestHandler<GetStockIssueByIdQuery, StockIssueDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStockIssueByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StockIssueDto?> Handle(GetStockIssueByIdQuery request, CancellationToken cancellationToken)
    {
        var issue = await _unitOfWork.Repository<StockIssue>().GetByIdAsync(request.Id);
        return issue is null ? null : _mapper.Map<StockIssueDto>(issue);
    }
}
