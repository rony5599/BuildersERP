using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.TestReports;

public record SetTestReportActiveCommand(long Id, bool IsActive) : IRequest<bool>;

public class SetTestReportActiveCommandHandler : IRequestHandler<SetTestReportActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetTestReportActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetTestReportActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<TestReport>();
        var item = await repository.GetByIdAsync(request.Id);
        if (item is null)
        {
            return false;
        }

        item.IsActive = request.IsActive;
        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
