using ClosedXML.Excel;

namespace BuilderERP.Application.Features.Reports.Export;

public class ExcelReportExporter
{
    public byte[] Export(string title, IReadOnlyList<ReportColumn> columns, IReadOnlyList<ReportRow> rows)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(title.Length > 31 ? title[..31] : title);

        for (var col = 0; col < columns.Count; col++)
        {
            var cell = worksheet.Cell(1, col + 1);
            cell.Value = columns[col].Header;
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGray;
        }

        for (var row = 0; row < rows.Count; row++)
        {
            for (var col = 0; col < columns.Count; col++)
            {
                worksheet.Cell(row + 2, col + 1).Value = rows[row].Cells.GetValueOrDefault(columns[col].Key) ?? string.Empty;
            }
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
