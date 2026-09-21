using BuilderERP.Application.Common;
using BuilderERP.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BuilderERP.Application.Features.GoodsReceives.Export;

public class GoodsReceivePdfExporter
{
    private const int ItemsPerPage = 16;

    public byte[] Export(GoodsReceivePrintDto data)
    {
        var pages = data.Lines
            .Select((line, index) => (line, index))
            .GroupBy(x => x.index / ItemsPerPage)
            .Select(g => g.Select(x => x.line).ToList())
            .ToList();

        if (pages.Count == 0)
        {
            pages.Add(new List<GoodsReceivePrintLineDto>());
        }

        var document = Document.Create(container =>
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
                        column.Item().AlignCenter().Text("GOODS RECEIVED NOTE").FontSize(22).Bold();
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

                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Supplier").Bold();
                                c.Item().Text(data.SupplierName);
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Warehouse").Bold();
                                c.Item().Text(data.WarehouseName);
                                c.Item().Text($"Project: {data.ProjectName}");
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().AlignRight().Text(t =>
                                {
                                    t.Span("GRN No: ").Bold();
                                    t.Span(data.GrnNumber);
                                });
                                c.Item().AlignRight().Text(t =>
                                {
                                    t.Span($"{data.SourceType} No: ").Bold();
                                    t.Span(data.SourceDocumentNumber);
                                });
                                c.Item().AlignRight().Text(t =>
                                {
                                    t.Span("Received Date: ").Bold();
                                    t.Span(data.ReceivedDate.ToString("MMM dd, yyyy"));
                                });
                                c.Item().AlignRight().Text(t =>
                                {
                                    t.Span("Status: ").Bold();
                                    t.Span(data.Status.ToString());
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
                                columns.ConstantColumn(24);
                                columns.RelativeColumn(2.5f);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1.3f);
                                columns.RelativeColumn(1.3f);
                                columns.RelativeColumn(1.4f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Sl.").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Description").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).AlignRight().Text("Qty").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("UOM").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).AlignRight().Text("Rate").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).AlignRight().Text("Amount").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Blue.Medium).Padding(5).Text("Batch/Serial").FontColor(Colors.White).Bold();
                            });

                            var sl = startSl;
                            foreach (var line in pageLines)
                            {
                                var batchSerial = string.Join(" / ", new[] { line.BatchNo, line.SerialNo }.Where(v => !string.IsNullOrWhiteSpace(v)));
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(sl.ToString());
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(line.Description);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(line.Quantity.ToString("0.##"));
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(line.UnitOfMeasure);
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{line.Rate:N2}");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{line.Amount:N2}");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(batchSerial);
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
                                    r.RelativeItem().Text("Total").Bold().FontSize(12);
                                    r.RelativeItem().AlignRight().Text($"{data.Total:N2}").Bold().FontSize(12);
                                });
                            });
                        });

                        column.Item().PaddingTop(6).Text($"In Words: {AmountInWords.ToBdtWords(data.Total)}").Bold().FontSize(10);

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
                                c.Item().AlignCenter().Text("Received By").Bold();
                            });
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().PaddingBottom(4).BorderBottom(1).BorderColor(Colors.Black).Height(30);
                                c.Item().AlignCenter().Text("Checked By").Bold();
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
        });

        return document.GeneratePdf();
    }
}
