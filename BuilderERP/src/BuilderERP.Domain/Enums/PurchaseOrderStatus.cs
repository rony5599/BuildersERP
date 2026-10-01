namespace BuilderERP.Domain.Enums;

public enum PurchaseOrderStatus
{
    Draft = 0,
    Approved = 1,
    PartiallyReceived = 2,
    Received = 3,
    Closed = 4,
    Cancelled = 5,
    Submitted = 6,
    AwaitingApproval = 7,
    Rejected = 8
}
