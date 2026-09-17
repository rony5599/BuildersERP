using BuilderERP.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BuilderERP.Application.Features.PurchaseOrders.Export;

public class PurchaseOrderPdfExporter
{
    private const int ItemsPerPage = 10;
    private static readonly string[] CopyLabels = { "Office Copy", "Customer Copy", "Bill Copy" };
    // Office Copy and Customer Copy are disabled for now — only Bill Copy prints. Kept above, not deleted.
    private static readonly string[] ActiveCopyLabels = CopyLabels.Where(l => l == "Bill Copy").ToArray();

    public byte[] Export(PurchaseOrderPrintDto data)
    {
        var pages = data.Lines
            .Select((line, index) => (line, index))
            .GroupBy(x => x.index / ItemsPerPage)
            .Select(g => g.Select(x => x.line).ToList())
            .ToList();

        if (pages.Count == 0)
        {
            pages.Add(new List<PurchaseOrderPrintLineDto>());
        }

        var document = Document.Create(container =>
        {
            foreach (var copyLabel in ActiveCopyLabels)
            {
                for (var pageIndex = 0; pageIndex < pages.Count; pageIndex++)
                {
                    var pageLines = pages[pageIndex];
                    var startSl = pageIndex * ItemsPerPage + 1;
                    var isLastPage = pageIndex == pages.Count - 1;

                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(36);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        page.Header().Column(column =>
                        {
                            column.Item().AlignRight().Border(1).BorderColor(Colors.Grey.Darken2).Padding(4).Text(copyLabel).Bold().FontSize(9);
                            column.Item().AlignCenter().Text("PURCHASE ORDER").FontSize(22).Bold();
                        column.Item().PaddingTop(12).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text(data.CompanyName).Bold().FontSize(13);
                                if (!string.IsNullOrWhiteSpace(data.CompanyAddress))
                                {
                                    c.Item().Text(data.CompanyAddress);
                                }
                                if (!string.IsNullOrWhiteSpace(data.CompanyPhone))
                                {
                                    c.Item().Text($"Mobile: {data.CompanyPhone}");
                                }
                                if (!string.IsNullOrWhiteSpace(data.CompanyEmail))
                                {
                                    c.Item().Text($"Email: {data.CompanyEmail}");
                                }
                            });
                        });

                        column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        if (!string.IsNullOrWhiteSpace(data.TermsOfPayment) || !string.IsNullOrWhiteSpace(data.DispatchedThrough) || !string.IsNullOrWhiteSpace(data.Destination))
                        {
                            column.Item().PaddingTop(10).Row(row =>
                            {
                                if (!string.IsNullOrWhiteSpace(data.TermsOfPayment))
                                {
                                    row.RelativeItem().Column(c =>
                                    {
                                        c.Item().Text("Terms of Payment / Delivery").Bold();
                                        c.Item().Text(data.TermsOfPayment);
                                    });
                                }
                                if (!string.IsNullOrWhiteSpace(data.DispatchedThrough))
                                {
                                    row.RelativeItem().Column(c =>
                                    {
                                        c.Item().Text("Dispatched Through").Bold();
                                        c.Item().Text(data.DispatchedThrough);
                                    });
                                }
                                if (!string.IsNullOrWhiteSpace(data.Destination))
                                {
                                    row.RelativeItem().Column(c =>
                                    {
                                        c.Item().Text("Destination").Bold();
                                        c.Item().Text(data.Destination);
                                    });
                                }
                            });
                            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        }

                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Consignee - Ship To").Bold();
                                c.Item().Text(data.CompanyName);
                                if (!string.IsNullOrWhiteSpace(data.CompanyAddress))
                                {
                                    c.Item().Text(data.CompanyAddress);
                                }
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Supplier").Bold();
                                c.Item().Text(data.SupplierName);
                                if (!string.IsNullOrWhiteSpace(data.SupplierAddress))
                                {
                                    c.Item().Text(data.SupplierAddress);
                                }
                                if (!string.IsNullOrWhiteSpace(data.SupplierPhone))
                                {
                                    c.Item().Text($"Phone: {data.SupplierPhone}");
                                }
                                if (!string.IsNullOrWhiteSpace(data.SupplierEmail))
                                {
                                    c.Item().Text($"Email: {data.SupplierEmail}");
                                }
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().AlignRight().Text(t =>
                                {
                                    t.Span("PO No: ").Bold();
                                    t.Span(data.PONumber);
                                });
                                if (!string.IsNullOrWhiteSpace(data.RequisitionNumber))
                                {
                                    c.Item().AlignRight().Text(t =>
                                    {
                                        t.Span("Req No: ").Bold();
                                        t.Span(data.RequisitionNumber);
                                    });
                                }
                                c.Item().AlignRight().Text(t =>
                                {
                                    t.Span("Order Date: ").Bold();
                                    t.Span(data.OrderDate.ToString("MMM dd, yyyy"));
                                });
                            });
                        });
                    });

                    page.Content().PaddingTop(16).Column(column =>
                    {
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(1.5f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Medium).Padding(6).Text("Sl.").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(6).Text("Description").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(6).AlignRight().Text("Qty").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(6).AlignRight().Text("Rate").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(6).AlignRight().Text("Amount").FontColor(Colors.White).Bold();
                            });

                            var sl = startSl;
                            foreach (var line in pageLines)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(sl.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(line.Description);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignRight().Text(line.Quantity.ToString("0.##"));
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignRight().Text($"{line.Rate:N2}");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignRight().Text($"{line.Amount:N2}");
                                sl++;
                            }
                        });

                        if (!isLastPage)
                        {
                            column.Item().PaddingTop(8).AlignRight().Text("Continued on next page...").Italic().FontSize(9);
                            return;
                        }

                        column.Item().PaddingTop(16).Row(row =>
                        {
                            row.RelativeItem();
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(4).Row(r =>
                                {
                                    r.RelativeItem().Text("Subtotal").Bold();
                                    r.RelativeItem().AlignRight().Text($"{data.Subtotal:N2}");
                                });
                                c.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(4).Row(r =>
                                {
                                    r.RelativeItem().Text("Total").Bold().FontSize(12);
                                    r.RelativeItem().AlignRight().Text($"{data.Total:N2}").Bold().FontSize(12);
                                });
                            });
                        });

                        if (!string.IsNullOrWhiteSpace(data.Remarks))
                        {
                            column.Item().PaddingTop(16).Column(c =>
                            {
                                c.Item().Text("Remarks").Bold();
                                c.Item().Text(data.Remarks);
                            });
                        }

                        column.Item().PaddingTop(60).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().PaddingBottom(4).BorderBottom(1).BorderColor(Colors.Black).Height(30);
                                c.Item().AlignCenter().Text("Sr. Executive").Bold();
                                c.Item().AlignCenter().Text("Procurement Dept.").FontSize(8);
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().PaddingBottom(4).BorderBottom(1).BorderColor(Colors.Black).Height(30);
                                c.Item().AlignCenter().Text("Sr. Manager").Bold();
                                c.Item().AlignCenter().Text("Procurement Dept.").FontSize(8);
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().PaddingBottom(4).BorderBottom(1).BorderColor(Colors.Black).Height(30);
                                c.Item().AlignCenter().Text("Authorized Signatory").Bold();
                                c.Item().AlignCenter().Text(data.CompanyName).FontSize(8);
                            });
                        });
                    });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                    });
                }
            }
        });

        return document.GeneratePdf();
    }
}
