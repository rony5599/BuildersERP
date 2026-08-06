using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.StockIssues;

public record SetStockIssueActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetStockIssueActiveCommandHandler : IRequestHandler<SetStockIssueActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetStockIssueActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetStockIssueActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<StockIssue>();
        var issue = await repository.GetByIdAsync(request.Id);
        if (issue is null)
        {
            return false;
        }

        issue.IsActive = request.IsActive;
        repository.Update(issue);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
