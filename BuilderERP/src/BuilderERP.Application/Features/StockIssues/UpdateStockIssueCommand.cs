using BuilderERP.Application.DTOs;
using BuilderERP.Application.Features.Stocks;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockIssues;

public record UpdateStockIssueCommand(UpdateStockIssueDto Dto) : IRequest<bool>;

public class UpdateStockIssueCommandHandler : IRequestHandler<UpdateStockIssueCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStockIssueCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateStockIssueCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<StockIssue>();
        var issue = await repository.GetByIdAsync(request.Dto.Id);
        if (issue is null)
        {
            return false;
        }

        var previousMaterialId = issue.MaterialId;
        var previousWarehouseId = issue.WarehouseId;
        var previousQuantity = issue.Quantity;

        issue.IssueNumber = request.Dto.IssueNumber;
        issue.IssueDate = request.Dto.IssueDate;
        issue.Quantity = request.Dto.Quantity;
        issue.IssuedTo = request.Dto.IssuedTo;
        issue.ConsumedQuantity = request.Dto.ConsumedQuantity;
        issue.WastageQuantity = request.Dto.WastageQuantity;
        issue.WastageReason = request.Dto.WastageReason;
        issue.Remarks = request.Dto.Remarks;
        issue.MaterialId = request.Dto.MaterialId;
        issue.WarehouseId = request.Dto.WarehouseId;

        repository.Update(issue);

        if (previousMaterialId == request.Dto.MaterialId && previousWarehouseId == request.Dto.WarehouseId)
        {
            var delta = previousQuantity - request.Dto.Quantity;
            await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, request.Dto.MaterialId, request.Dto.WarehouseId, delta);
        }
        else
        {
            await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, previousMaterialId, previousWarehouseId, previousQuantity);
            await StockSync.ApplyQuantityDeltaAsync(_unitOfWork, request.Dto.MaterialId, request.Dto.WarehouseId, -request.Dto.Quantity);
        }

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
