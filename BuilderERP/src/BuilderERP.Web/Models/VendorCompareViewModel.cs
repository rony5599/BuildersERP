using BuilderERP.Application.DTOs;

namespace BuilderERP.Web.Models;

public class VendorCompareViewModel
{
    public long? SelectedRfqId { get; set; }
    public RfqDto? Rfq { get; set; }
    public IReadOnlyList<RfqDto> Rfqs { get; set; } = Array.Empty<RfqDto>();
    public IReadOnlyList<VendorQuotationDto> Quotations { get; set; } = Array.Empty<VendorQuotationDto>();
}
