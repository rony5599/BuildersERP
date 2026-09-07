using BuilderERP.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BuilderERP.Application.Features.EngineerWorkOrders.Export;

public class EngineerWorkOrderPdfExporter
{
    private const int ItemsPerPage = 10;
    private static readonly string[] CopyLabels = { "Office Copy", "Supplier Copy", "Site Copy" };

    public byte[] Export(EngineerWorkOrderPrintDto data)
    {
        var pages = data.Lines
            .Select((line, index) => (line, index))
            .GroupBy(x => x.index / ItemsPerPage)
            .Select(g => g.Select(x => x.line).ToList())
            .ToList();

        if (pages.Count == 0)
        {
            pages.Add(new List<EngineerWorkOrderPrintLineDto>());
        }

        var document = Document.Create(container =>
        {
            foreach (var copyLabel in CopyLabels)
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
                            column.Item().AlignCenter().Text("ENGINEER WORK ORDER").FontSize(22).Bold();
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

                            if (!string.IsNullOrWhiteSpace(data.TermsAndCondition))
                            {
                                column.Item().PaddingTop(10).Column(c =>
                                {
                                    c.Item().Text("Terms & Condition").Bold();
                                    c.Item().Text(data.TermsAndCondition);
                                });
                                column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            }

                            column.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text("Requisition").Bold();
                                    c.Item().Text(data.RequisitionNumber);
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
                                        t.Span("WO No: ").Bold();
                                        t.Span(data.WorkOrderNo);
                                    });
                                    c.Item().AlignRight().Text(t =>
                                    {
                                        t.Span("Rev No: ").Bold();
                                        t.Span(data.RevisionNo.ToString());
                                    });
                                    c.Item().AlignRight().Text(t =>
                                    {
                                        t.Span("Date: ").Bold();
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
                                    columns.RelativeColumn(3.5f);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(1.2f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Blue.Medium).Padding(6).Text("Sl.").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Medium).Padding(6).Text("Description").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Medium).Padding(6).Text("Unit").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Medium).Padding(6).AlignRight().Text("Qty").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Medium).Padding(6).AlignRight().Text("Rate").FontColor(Colors.White).Bold();
                                    header.Cell().Background(Colors.Blue.Medium).Padding(6).AlignRight().Text("Amount").FontColor(Colors.White).Bold();
                                });

                                var sl = startSl;
                                foreach (var line in pageLines)
                                {
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(sl.ToString());
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(line.Description);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(line.UnitOfMeasure);
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

                            column.Item().PaddingTop(60).Row(row =>
                            {
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().PaddingBottom(4).BorderBottom(1).BorderColor(Colors.Black).Height(30);
                                    c.Item().AlignCenter().Text("Sr. Engineer").Bold();
                                    c.Item().AlignCenter().Text("Engineering Dept.").FontSize(8);
                                });
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().PaddingBottom(4).BorderBottom(1).BorderColor(Colors.Black).Height(30);
                                    c.Item().AlignCenter().Text("Project Manager").Bold();
                                    c.Item().AlignCenter().Text("Engineering Dept.").FontSize(8);
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
