using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DefectRecords;

public record SetDefectRecordActiveCommand(Guid Id, bool IsActive) : IRequest<bool>;

public class SetDefectRecordActiveCommandHandler : IRequestHandler<SetDefectRecordActiveCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetDefectRecordActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SetDefectRecordActiveCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<DefectRecord>();
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
