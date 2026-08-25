namespace BuilderERP.Domain.Entities;

public class SafetyTraining : BaseEntity
{
    public long WorkerId { get; set; }
    public Worker Worker { get; set; } = null!;

    public string TrainingTitle { get; set; } = string.Empty;
    public DateTime TrainingDate { get; set; } = DateTime.UtcNow;
    public string? TrainerName { get; set; }
    public decimal DurationHours { get; set; }
    public string? CertificateNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
}
