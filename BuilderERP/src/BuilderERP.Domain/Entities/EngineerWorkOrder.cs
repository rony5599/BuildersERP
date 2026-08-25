using BuilderERP.Domain.Enums;

namespace BuilderERP.Domain.Entities;

public class EngineerWorkOrder : BaseEntity
{
    public string WorkOrderNo { get; set; } = string.Empty;

    public long EngineerWorkOrderRequisitionId { get; set; }
    public EngineerWorkOrderRequisition EngineerWorkOrderRequisition { get; set; } = null!;

    public long? MotherWorkOrderId { get; set; }
    public EngineerWorkOrder? MotherWorkOrder { get; set; }
    public ICollection<EngineerWorkOrder> Revisions { get; set; } = new List<EngineerWorkOrder>();

    public int RevisionNo { get; set; }
    public bool IsLatestRevision { get; set; }
    public DateTime? RevisionDate { get; set; }
    public string? TermsAndCondition { get; set; }

    public long SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public EngineerWorkOrderStatus Status { get; set; } = EngineerWorkOrderStatus.Draft;
    public decimal TotalAmount { get; set; }

    public long? PreviousWorkOrderId { get; set; }
    public EngineerWorkOrder? PreviousWorkOrder { get; set; }

    public ICollection<EngineerWorkOrderDetail> Details { get; set; } = new List<EngineerWorkOrderDetail>();
}
