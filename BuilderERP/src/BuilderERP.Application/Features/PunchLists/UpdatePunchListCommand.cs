using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.PunchLists;

public record UpdatePunchListCommand(UpdatePunchListDto Dto) : IRequest<bool>;

public class UpdatePunchListCommandHandler : IRequestHandler<UpdatePunchListCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePunchListCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdatePunchListCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<PunchList>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.ItemNumber = request.Dto.ItemNumber;
        item.Location = request.Dto.Location;
        item.Description = request.Dto.Description;
        item.Status = request.Dto.Status;
        item.AssignedTo = request.Dto.AssignedTo;
        item.DueDate = request.Dto.DueDate;
        item.CompletedDate = request.Dto.CompletedDate;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
