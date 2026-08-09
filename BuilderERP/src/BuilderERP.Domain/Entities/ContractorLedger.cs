namespace BuilderERP.Domain.Entities;

public class ContractorLedger : BaseEntity
{
    public Guid ContractorId { get; set; }
    public Contractor Contractor { get; set; } = null!;

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; } = true;
}
