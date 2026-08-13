using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BuilderERP.Application.Features.Reports.Export;

public class PdfReportExporter
{
    public byte[] Export(string title, IReadOnlyList<ReportColumn> columns, IReadOnlyList<ReportRow> rows)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(24);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Header().Text(title).FontSize(16).Bold();

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columnsDefinition =>
                    {
                        foreach (var _ in columns)
                        {
                            columnsDefinition.RelativeColumn();
                        }
                    });

                    table.Header(header =>
                    {
                        foreach (var column in columns)
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text(column.Header).Bold();
                        }
                    });

                    foreach (var row in rows)
                    {
                        foreach (var column in columns)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4)
                                .Text(row.Cells.GetValueOrDefault(column.Key) ?? string.Empty);
                        }
                    }
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
