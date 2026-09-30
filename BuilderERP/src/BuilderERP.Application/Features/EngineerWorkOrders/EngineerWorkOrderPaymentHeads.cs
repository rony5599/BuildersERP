using BuilderERP.Application.DTOs;
using BuilderERP.Domain.Entities;

namespace BuilderERP.Application.Features.EngineerWorkOrders;

public static class EngineerWorkOrderPaymentHeads
{
    // The form always posts a blank template row; drop rows the user left empty.
    public static List<EngineerWorkOrderPaymentHeadDto> Clean(IEnumerable<EngineerWorkOrderPaymentHeadDto>? heads) =>
        (heads ?? Enumerable.Empty<EngineerWorkOrderPaymentHeadDto>())
            .Where(h => !string.IsNullOrWhiteSpace(h.HeadName) || h.Percent != 0)
            .Select(h => new EngineerWorkOrderPaymentHeadDto { HeadName = h.HeadName?.Trim() ?? string.Empty, Percent = h.Percent })
            .ToList();

    public static IEnumerable<EngineerWorkOrderPaymentHead> ToEntities(IEnumerable<EngineerWorkOrderPaymentHeadDto> heads, long workOrderId) =>
        Clean(heads).Select((h, i) => new EngineerWorkOrderPaymentHead
        {
            EngineerWorkOrderId = workOrderId,
            HeadName = h.HeadName,
            Percent = h.Percent,
            SortOrder = i + 1
        });

    public static string Key(string headName) => headName.Trim().ToUpperInvariant();
}
