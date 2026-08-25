using AutoMapper;
using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Stocks;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockIssues;

public record CreateStockIssueCommand(CreateStockIssueDto Dto) : IRequest<long>;

public class CreateStockIssueCommandHandler : IRequestHandler<CreateStockIssueCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateStockIssueCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<long> Handle(CreateStockIssueCommand request, CancellationToken cancellationToken)
    {
        var issue = _mapper.Map<StockIssue>(request.Dto);
        await _unitOfWork.Repository<StockIssue>().AddAsync(issue);

        await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, issue.MaterialId, issue.WarehouseId, -issue.Quantity, InventoryTransactionType.StockAdjustment, issue.IssueNumber);

        await _unitOfWork.SaveChangesAsync();

        return issue.Id;
    }
}
