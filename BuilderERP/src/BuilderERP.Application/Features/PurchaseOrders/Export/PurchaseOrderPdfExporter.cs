using BuilderERP.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BuilderERP.Application.Features.PurchaseOrders.Export;

public class PurchaseOrderPdfExporter
{
    public byte[] Export(PurchaseOrderPrintDto data)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
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
                });

                page.Content().PaddingTop(16).Column(column =>
                {
                    column.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    column.Item().Row(row =>
                    {
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
                            c.Item().AlignRight().Text(t =>
                            {
                                t.Span("Order Date: ").Bold();
                                t.Span(data.OrderDate.ToString("MMM dd, yyyy"));
                            });
                        });
                    });

                    column.Item().PaddingTop(16).Table(table =>
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

                        var sl = 1;
                        foreach (var line in data.Lines)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(sl.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(line.Description);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignRight().Text(line.Quantity.ToString("0.##"));
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignRight().Text($"$ {line.Rate:N2}");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignRight().Text($"$ {line.Amount:N2}");
                            sl++;
                        }
                    });

                    column.Item().PaddingTop(16).Row(row =>
                    {
                        row.RelativeItem();
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(4).Row(r =>
                            {
                                r.RelativeItem().Text("Subtotal").Bold();
                                r.RelativeItem().AlignRight().Text($"$ {data.Subtotal:N2}");
                            });
                            c.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingTop(4).Row(r =>
                            {
                                r.RelativeItem().Text("Total").Bold().FontSize(12);
                                r.RelativeItem().AlignRight().Text($"$ {data.Total:N2}").Bold().FontSize(12);
                            });
                        });
                    });

                    column.Item().PaddingTop(60).Row(row =>
                    {
                        row.RelativeItem();
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().PaddingBottom(4).BorderBottom(1).BorderColor(Colors.Black).Height(30);
                            c.Item().AlignCenter().Text("Authorized Signatory");
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
        });

        return document.GeneratePdf();
    }
}
