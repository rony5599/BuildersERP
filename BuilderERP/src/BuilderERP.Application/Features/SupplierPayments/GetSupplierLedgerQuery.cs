using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.SupplierPayments;

public record GetSupplierLedgerQuery(long SupplierId, DateTime? DateFrom = null, DateTime? DateTo = null) : IRequest<SupplierLedgerDto?>;

// Computed live from approved PO bills (credit), approved/completed purchase returns (debit)
// and supplier payments (debit); nothing is stored, so it can never drift from the source documents.
public class GetSupplierLedgerQueryHandler : IRequestHandler<GetSupplierLedgerQuery, SupplierLedgerDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSupplierLedgerQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private record Entry(DateTime Date, long Order, string Type, string DocumentNumber, string Reference, decimal Debit, decimal Credit);

    public async Task<SupplierLedgerDto?> Handle(GetSupplierLedgerQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _unitOfWork.Repository<Supplier>().Query()
            .FirstOrDefaultAsync(s => s.Id == request.SupplierId, cancellationToken);
        if (supplier is null)
        {
            return null;
        }

        var entries = new List<Entry>();

        var bills = await _unitOfWork.Repository<PoBill>().Query()
            .Where(b => b.IsActive && b.Status == PoBillStatus.Approved
                        && b.PurchaseOrder.VendorQuotation.SupplierId == request.SupplierId)
            .Select(b => new { b.Id, b.BillDate, b.BillNumber, b.SupplierInvoiceNumber, PO = b.PurchaseOrder.PONumber, b.TotalAmount })
            .ToListAsync(cancellationToken);
        entries.AddRange(bills.Select(b => new Entry(b.BillDate.Date, b.Id, "Bill", b.BillNumber,
            string.IsNullOrWhiteSpace(b.SupplierInvoiceNumber) ? $"PO {b.PO}" : $"PO {b.PO} / Inv {b.SupplierInvoiceNumber}", 0, b.TotalAmount)));

        var returns = await _unitOfWork.Repository<PurchaseReturn>().Query()
            .Where(r => r.IsActive
                        && (r.Status == PurchaseReturnStatus.Approved || r.Status == PurchaseReturnStatus.Completed)
                        && r.GoodsReceive.PurchaseOrder != null
                        && r.GoodsReceive.PurchaseOrder.VendorQuotation.SupplierId == request.SupplierId)
            .Select(r => new { r.Id, r.ReturnDate, r.ReturnNumber, PO = r.GoodsReceive.PurchaseOrder!.PONumber, r.ReturnAmount })
            .ToListAsync(cancellationToken);
        entries.AddRange(returns.Select(r => new Entry(r.ReturnDate.Date, r.Id, "Purchase Return", r.ReturnNumber, $"PO {r.PO}", r.ReturnAmount, 0)));

        var payments = await _unitOfWork.Repository<SupplierPayment>().Query()
            .Where(p => p.IsActive && p.SupplierId == request.SupplierId)
            .Select(p => new { p.Id, p.PaymentDate, p.PaymentNumber, p.Method, p.ReferenceNumber, Bill = p.PoBill.BillNumber, p.Amount })
            .ToListAsync(cancellationToken);
        entries.AddRange(payments.Select(p => new Entry(p.PaymentDate.Date, p.Id, "Payment", p.PaymentNumber,
            $"Bill {p.Bill} / {p.Method}" + (string.IsNullOrWhiteSpace(p.ReferenceNumber) ? "" : $" / Ref {p.ReferenceNumber}"), p.Amount, 0)));

        var from = request.DateFrom?.Date;
        var to = request.DateTo?.Date;

        var opening = from.HasValue
            ? entries.Where(e => e.Date < from.Value).Sum(e => e.Credit - e.Debit)
            : 0m;

        var inRange = entries
            .Where(e => (!from.HasValue || e.Date >= from.Value) && (!to.HasValue || e.Date <= to.Value))
            .OrderBy(e => e.Date).ThenBy(e => e.Type == "Bill" ? 0 : 1).ThenBy(e => e.Order)
            .ToList();

        var ledger = new SupplierLedgerDto
        {
            SupplierId = supplier.Id,
            SupplierName = supplier.Name,
            SupplierAddress = supplier.Address,
            SupplierPhone = supplier.Phone,
            DateFrom = from,
            DateTo = to,
            OpeningBalance = opening
        };

        var running = opening;
        foreach (var e in inRange)
        {
            running += e.Credit - e.Debit;
            ledger.Rows.Add(new SupplierLedgerRowDto
            {
                Date = e.Date,
                Type = e.Type,
                DocumentNumber = e.DocumentNumber,
                Reference = e.Reference,
                Debit = e.Debit,
                Credit = e.Credit,
                Balance = running
            });
        }

        ledger.CompanyName = await _unitOfWork.Repository<Company>().Query()
            .Where(c => c.IsActive).Select(c => c.Name).FirstOrDefaultAsync(cancellationToken);

        return ledger;
    }
}
