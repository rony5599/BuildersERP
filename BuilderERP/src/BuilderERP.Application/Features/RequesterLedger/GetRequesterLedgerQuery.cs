using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuilderERP.Application.Features.RequesterLedger;

public record GetRequesterLedgerQuery(long EmployeeId, DateTime? DateFrom = null, DateTime? DateTo = null) : IRequest<RequesterLedgerDto?>;

// Computed live: cash disbursements are debits (cash issued to the requester),
// approved cash PO bills are credits (cash spent and accounted for). Nothing is stored.
public class GetRequesterLedgerQueryHandler : IRequestHandler<GetRequesterLedgerQuery, RequesterLedgerDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRequesterLedgerQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private record Entry(DateTime Date, long Order, int Sort, string Type, string DocumentNumber, string Reference, decimal Debit, decimal Credit);

    public async Task<RequesterLedgerDto?> Handle(GetRequesterLedgerQuery request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.Repository<Employee>().Query()
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);
        if (employee is null)
        {
            return null;
        }

        var entries = new List<Entry>();

        var disbursements = await _unitOfWork.Repository<CashDisbursement>().Query()
            .Where(d => d.IsActive && d.RequesterEmployeeId == request.EmployeeId)
            .Select(d => new { d.Id, d.DisbursementDate, d.DisbursementNumber, d.Method, d.ReferenceNumber, Requisition = d.CashRequisition.RequisitionNumber, d.Amount })
            .ToListAsync(cancellationToken);
        entries.AddRange(disbursements.Select(d => new Entry(d.DisbursementDate.Date, d.Id, 0, "Cash Issued", d.DisbursementNumber,
            $"Req {d.Requisition} / {d.Method}" + (string.IsNullOrWhiteSpace(d.ReferenceNumber) ? "" : $" / Ref {d.ReferenceNumber}"), d.Amount, 0)));

        var bills = await _unitOfWork.Repository<CashPoBill>().Query()
            .Where(b => b.IsActive && b.Status == PoBillStatus.Approved && b.RequesterEmployeeId == request.EmployeeId)
            .Select(b => new
            {
                b.Id, b.BillDate, b.BillNumber, b.MemoNumber, b.TotalAmount,
                CPO = b.CashPurchaseOrder.CPONumber,
                Supplier = b.CashPurchaseOrder.Supplier.Name,
                Requisition = b.CashPurchaseOrder.CashRequisition.RequisitionNumber
            })
            .ToListAsync(cancellationToken);
        entries.AddRange(bills.Select(b => new Entry(b.BillDate.Date, b.Id, 1, "Cash Bill", b.BillNumber,
            $"CPO {b.CPO} / {b.Supplier} / Req {b.Requisition}" + (string.IsNullOrWhiteSpace(b.MemoNumber) ? "" : $" / Memo {b.MemoNumber}"), 0, b.TotalAmount)));

        var from = request.DateFrom?.Date;
        var to = request.DateTo?.Date;

        var opening = from.HasValue
            ? entries.Where(e => e.Date < from.Value).Sum(e => e.Debit - e.Credit)
            : 0m;

        var inRange = entries
            .Where(e => (!from.HasValue || e.Date >= from.Value) && (!to.HasValue || e.Date <= to.Value))
            .OrderBy(e => e.Date).ThenBy(e => e.Sort).ThenBy(e => e.Order)
            .ToList();

        var ledger = new RequesterLedgerDto
        {
            EmployeeId = employee.Id,
            EmployeeName = employee.EmployeeName,
            EmployeeCode = employee.EmployeeCode,
            EmployeeMobile = employee.MobileNo,
            DateFrom = from,
            DateTo = to,
            OpeningBalance = opening
        };

        var running = opening;
        foreach (var e in inRange)
        {
            running += e.Debit - e.Credit;
            ledger.Rows.Add(new RequesterLedgerRowDto
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

// Employees that have raised at least one cash requisition, for the ledger's requester picker.
public record GetRequesterOptionsQuery : IRequest<IReadOnlyList<RequesterOptionDto>>;

public class GetRequesterOptionsQueryHandler : IRequestHandler<GetRequesterOptionsQuery, IReadOnlyList<RequesterOptionDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRequesterOptionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<RequesterOptionDto>> Handle(GetRequesterOptionsQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<CashRequisition>().Query()
            .Select(r => new RequesterOptionDto { Id = r.RequesterEmployeeId, Name = r.RequesterEmployee.EmployeeName })
            .Distinct()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
