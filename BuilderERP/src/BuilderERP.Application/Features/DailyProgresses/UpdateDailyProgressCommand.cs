using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.DailyProgresses;

public record UpdateDailyProgressCommand(UpdateDailyProgressDto Dto) : IRequest<bool>;

public class UpdateDailyProgressCommandHandler : IRequestHandler<UpdateDailyProgressCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDailyProgressCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateDailyProgressCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<DailyProgress>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.ProgressDate = request.Dto.ProgressDate;
        item.Description = request.Dto.Description;
        item.PercentComplete = request.Dto.PercentComplete;
        item.ManpowerCount = request.Dto.ManpowerCount;
        item.Remarks = request.Dto.Remarks;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
