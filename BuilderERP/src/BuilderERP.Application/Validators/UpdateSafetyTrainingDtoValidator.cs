using BuilderERP.Application.DTOs;
using FluentValidation;

namespace BuilderERP.Application.Validators;

public class UpdateSafetyTrainingDtoValidator : AbstractValidator<UpdateSafetyTrainingDto>
{
    public UpdateSafetyTrainingDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.WorkerId).NotEmpty();
        RuleFor(x => x.TrainingTitle).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TrainerName).MaximumLength(200);
        RuleFor(x => x.DurationHours).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CertificateNumber).MaximumLength(100);
    }
}
