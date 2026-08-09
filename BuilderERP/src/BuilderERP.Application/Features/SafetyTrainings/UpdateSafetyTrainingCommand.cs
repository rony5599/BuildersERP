using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Interfaces;
using MediatR;

namespace BuilderERP.Application.Features.SafetyTrainings;

public record UpdateSafetyTrainingCommand(UpdateSafetyTrainingDto Dto) : IRequest<bool>;

public class UpdateSafetyTrainingCommandHandler : IRequestHandler<UpdateSafetyTrainingCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSafetyTrainingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateSafetyTrainingCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<SafetyTraining>();
        var training = await repository.GetByIdAsync(request.Dto.Id);
        if (training is null)
        {
            return false;
        }

        training.WorkerId = request.Dto.WorkerId;
        training.TrainingTitle = request.Dto.TrainingTitle;
        training.TrainingDate = request.Dto.TrainingDate;
        training.TrainerName = request.Dto.TrainerName;
        training.DurationHours = request.Dto.DurationHours;
        training.CertificateNumber = request.Dto.CertificateNumber;
        training.ExpiryDate = request.Dto.ExpiryDate;

        repository.Update(training);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
