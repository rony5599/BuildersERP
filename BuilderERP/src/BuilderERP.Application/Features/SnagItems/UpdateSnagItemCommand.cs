using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SnagItems;

public record UpdateSnagItemCommand(UpdateSnagItemDto Dto) : IRequest<bool>;

public class UpdateSnagItemCommandHandler : IRequestHandler<UpdateSnagItemCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSnagItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSnagItemCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SnagItem>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.SnagNumber = request.Dto.SnagNumber;
        item.Description = request.Dto.Description;
        item.Location = request.Dto.Location;
        item.Severity = request.Dto.Severity;
        item.Status = request.Dto.Status;
        item.ReportedDate = request.Dto.ReportedDate;
        item.ResolvedDate = request.Dto.ResolvedDate;
        item.Remarks = request.Dto.Remarks;
        item.PropertyUnitId = request.Dto.PropertyUnitId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
