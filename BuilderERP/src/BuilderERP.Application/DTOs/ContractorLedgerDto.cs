namespace BuilderERP.Application.DTOs;

public class ContractorLedgerDto
{
    public Guid Id { get; set; }
    public Guid ContractorId { get; set; }
    public string ContractorName { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
}

public class CreateContractorLedgerDto
{
    public Guid ContractorId { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
}

public class UpdateContractorLedgerDto
{
    public Guid Id { get; set; }
    public Guid ContractorId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
}
