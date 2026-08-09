using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.Ncrs;

public record UpdateNcrCommand(UpdateNcrDto Dto) : IRequest<bool>;

public class UpdateNcrCommandHandler : IRequestHandler<UpdateNcrCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNcrCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateNcrCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Ncr>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.NcrNumber = request.Dto.NcrNumber;
        item.RaisedDate = request.Dto.RaisedDate;
        item.Description = request.Dto.Description;
        item.Severity = request.Dto.Severity;
        item.Status = request.Dto.Status;
        item.ResolutionDescription = request.Dto.ResolutionDescription;
        item.ClosedDate = request.Dto.ClosedDate;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
