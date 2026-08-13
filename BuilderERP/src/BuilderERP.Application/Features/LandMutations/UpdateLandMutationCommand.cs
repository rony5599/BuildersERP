using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.LandMutations;

public record UpdateLandMutationCommand(UpdateLandMutationDto Dto) : IRequest<bool>;

public class UpdateLandMutationCommandHandler : IRequestHandler<UpdateLandMutationCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLandMutationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateLandMutationCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<LandMutation>();
        var item = await repository.GetByIdAsync(request.Dto.Id);
        if (item is null)
        {
            return false;
        }

        item.MutationNumber = request.Dto.MutationNumber;
        item.ApplicantName = request.Dto.ApplicantName;
        item.KhatianNumber = request.Dto.KhatianNumber;
        item.DagNumber = request.Dto.DagNumber;
        item.MutationDate = request.Dto.MutationDate;
        item.Status = request.Dto.Status;
        item.Remarks = request.Dto.Remarks;
        item.ProjectId = request.Dto.ProjectId;

        repository.Update(item);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
