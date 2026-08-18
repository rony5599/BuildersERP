namespace BuilderERP.Domain.Entities;

public class CollectionTarget : BaseEntity
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TargetAmount { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid CollectionOfficerId { get; set; }
    public ApplicationUser CollectionOfficer { get; set; } = null!;
}
