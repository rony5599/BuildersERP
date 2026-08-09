namespace BuilderERP.Application.DTOs;

public class SafetyTrainingDto
{
    public Guid Id { get; set; }
    public Guid WorkerId { get; set; }
    public string WorkerName { get; set; } = string.Empty;
    public string TrainingTitle { get; set; } = string.Empty;
    public DateTime TrainingDate { get; set; }
    public string? TrainerName { get; set; }
    public decimal DurationHours { get; set; }
    public string? CertificateNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreateSafetyTrainingDto
{
    public Guid WorkerId { get; set; }
    public string TrainingTitle { get; set; } = string.Empty;
    public DateTime TrainingDate { get; set; } = DateTime.UtcNow;
    public string? TrainerName { get; set; }
    public decimal DurationHours { get; set; }
    public string? CertificateNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class UpdateSafetyTrainingDto
{
    public Guid Id { get; set; }
    public Guid WorkerId { get; set; }
    public string TrainingTitle { get; set; } = string.Empty;
    public DateTime TrainingDate { get; set; }
    public string? TrainerName { get; set; }
    public decimal DurationHours { get; set; }
    public string? CertificateNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
