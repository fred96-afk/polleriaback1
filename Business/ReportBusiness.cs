using DbModel;
using DbModel.Tables;
using ClosedXML.Excel;
using IBusiness;
using IRepository;
using Models.Reports;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.IO.Font.Constants;
using iText.Kernel.Font;

namespace Business;

public class ReportBusiness(IOrderRepository orderRepository, IClientRepository clientRepository) : IReportBusiness
{
    public async Task<byte[]> GenerateSalesPdfAsync(SalesReportRequest request)
    {
        var orders = await orderRepository.GetByDateRangeAsync(request.StartDate, request.EndDate);
        var orderList = orders.ToList();
        var productSales = AggregateProductSales(orderList);

        using var stream = new MemoryStream();
        using var writer = new PdfWriter(stream);
        using var pdf = new PdfDocument(writer);
        using var document = new Document(pdf, PageSize.A4);

        var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        var italicFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_OBLIQUE);

        // Header
        document.Add(new Paragraph("POLLERÍA EL GIGANTE")
            .SetFontSize(20)
            .SetFont(boldFont)
            .SetFontColor(DeviceRgb.RED)
            .SetTextAlignment(TextAlignment.CENTER));

        document.Add(new Paragraph("Reporte Estadístico de Ventas por Producto")
            .SetFontSize(14)
            .SetTextAlignment(TextAlignment.CENTER));

        document.Add(new Paragraph($"Periodo: {request.StartDate:dd/MM/yyyy} - {request.EndDate:dd/MM/yyyy}")
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.CENTER));

        document.Add(new Paragraph($"Fecha de Emisión: {PeruTimeHelper.Now:dd/MM/yyyy HH:mm}")
            .SetFontSize(8)
            .SetTextAlignment(TextAlignment.RIGHT)
            .SetMarginBottom(10));

        document.Add(new Paragraph("Nota: Unidades convertidas (2 Medios = 1 Entero, 4 Cuartos = 1 Entero)")
            .SetFontSize(8)
            .SetFont(italicFont)
            .SetMarginBottom(10));

        // Resumen
        var totalAmount = orderList.Sum(o => o.TotalAmount);
        var mostSold = productSales.OrderByDescending(x => x.Quantity).FirstOrDefault();
        
        Table summaryTable = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth().SetMarginBottom(20);
        
        summaryTable.AddCell(CreateSummaryCell("PRODUCTO MÁS VENDIDO", mostSold?.ProductName ?? "N/A", $"{mostSold?.Quantity:F2} unidades eq.", boldFont));
        summaryTable.AddCell(CreateSummaryCell("TOTAL RECAUDADO", $"S/ {totalAmount:F2}", $"{orderList.Count} pedidos", boldFont));
        summaryTable.AddCell(CreateSummaryCell("PRODUCTOS DIFERENTES", productSales.Count.ToString(), "Vendidos en el periodo", boldFont));
        
        document.Add(summaryTable);

        // Detalle Table
        document.Add(new Paragraph("Detalle de Ventas por Producto")
            .SetFontSize(12)
            .SetFont(boldFont)
            .SetMarginBottom(5));

        Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 4, 2, 3 })).UseAllAvailableWidth();
        
        table.AddHeaderCell(new Cell().Add(new Paragraph("#").SetFont(boldFont)).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Producto").SetFont(boldFont)).SetBackgroundColor(ColorConstants.LIGHT_GRAY));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Cant. (Eq.)").SetFont(boldFont)).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.RIGHT));
        table.AddHeaderCell(new Cell().Add(new Paragraph("Total S/").SetFont(boldFont)).SetBackgroundColor(ColorConstants.LIGHT_GRAY).SetTextAlignment(TextAlignment.RIGHT));

        int i = 1;
        if (!productSales.Any())
        {
            table.AddCell(new Cell(1, 4).Add(new Paragraph("No se encontraron ventas en este periodo.").SetFont(italicFont)).SetTextAlignment(TextAlignment.CENTER));
        }

        foreach (var item in productSales.OrderByDescending(x => x.Quantity))
        {
            table.AddCell(new Cell().Add(new Paragraph(i++.ToString())));
            table.AddCell(new Cell().Add(new Paragraph(item.ProductName)));
            table.AddCell(new Cell().Add(new Paragraph(item.Quantity.ToString("F2"))).SetTextAlignment(TextAlignment.RIGHT));
            table.AddCell(new Cell().Add(new Paragraph($"S/ {item.Total:F2}")).SetTextAlignment(TextAlignment.RIGHT));
        }

        document.Add(table);
        document.Close();

        return stream.ToArray();
    }

    private Cell CreateSummaryCell(string title, string value, string detail, PdfFont boldFont)
    {
        Cell cell = new Cell().SetPadding(5).SetBorder(new iText.Layout.Borders.SolidBorder(ColorConstants.GRAY, 0.5f));
        cell.Add(new Paragraph(title).SetFontSize(8).SetFontColor(ColorConstants.GRAY));
        cell.Add(new Paragraph(value).SetFontSize(12).SetFont(boldFont));
        cell.Add(new Paragraph(detail).SetFontSize(9));
        return cell;
    }

    public async Task<byte[]> GenerateSalesExcelAsync(SalesReportRequest request)
    {
        var orders = await orderRepository.GetByDateRangeAsync(request.StartDate, request.EndDate);
        var orderList = orders.ToList();

        var productSales = AggregateProductSales(orderList);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ventas por Producto");

        // Title
        var titleRange = worksheet.Range("A1:C1");
        titleRange.Merge().Value = "REPORTE DE VENTAS POR PRODUCTO - POLLERÍA EL GIGANTE";
        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 16;
        titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Date Range
        worksheet.Cell("A2").Value = $"Periodo: {request.StartDate:dd/MM/yyyy} - {request.EndDate:dd/MM/yyyy}";
        worksheet.Range("A2:C2").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        
        worksheet.Cell("A3").Value = "Nota: Unidades convertidas (2 Medios = 1 Entero, 4 Cuartos = 1 Entero)";
        worksheet.Range("A3:C3").Merge().Style.Font.Italic = true;

        // Headers
        var headers = new[] { "Producto", "Cant. Equivalente", "Total Recaudado (S/)" };
        for (int h = 0; h < headers.Length; h++)
        {
            var cell = worksheet.Cell(5, h + 1);
            cell.Value = headers[h];
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#D32F2F");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Font.Bold = true;
        }

        // Data
        int row = 6;
        foreach (var item in productSales.OrderByDescending(x => x.Quantity))
        {
            worksheet.Cell(row, 1).Value = item.ProductName;
            worksheet.Cell(row, 2).Value = item.Quantity;
            worksheet.Cell(row, 3).Value = item.Total;
            row++;
        }

        // Summary
        int lastRow = row;
        worksheet.Cell(lastRow, 2).Value = "TOTAL FINAL:";
        worksheet.Cell(lastRow, 2).Style.Font.Bold = true;
        worksheet.Cell(lastRow, 3).FormulaA1 = $"=SUM(C6:C{lastRow - 1})";
        worksheet.Cell(lastRow, 3).Style.Font.Bold = true;
        worksheet.Cell(lastRow, 3).Style.NumberFormat.Format = "S/ #,##0.00";

        // Formatting
        worksheet.Columns().AdjustToContents();
        worksheet.Range(5, 1, lastRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        worksheet.Range(5, 1, lastRow, 3).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        worksheet.Column(3).Style.NumberFormat.Format = "S/ #,##0.00";

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private List<AggregatedProductSales> AggregateProductSales(List<Order> orders)
    {
        var rawDetails = orders.SelectMany(o => o.OrderDetails).ToList();
        var aggregated = new Dictionary<string, AggregatedProductSales>();

        const string wholeChickenKey = "Pollo a la Brasa (Entero)";

        foreach (var detail in rawDetails)
        {
            string originalName = detail.Product?.Name ?? $"Producto {detail.ProductId}";
            decimal quantity = detail.Quantity;
            decimal subtotal = detail.Subtotal;
            string targetName = originalName;

            // Lógica de conversión
            if (originalName.Contains("Medio Pollo", StringComparison.OrdinalIgnoreCase))
            {
                targetName = wholeChickenKey;
                quantity = quantity * 0.5m; // 2 medios = 1 entero
            }
            else if (originalName.Contains("1/4 de Pollo", StringComparison.OrdinalIgnoreCase))
            {
                targetName = wholeChickenKey;
                quantity = quantity * 0.25m; // 4 cuartos = 1 entero
            }

            if (aggregated.ContainsKey(targetName))
            {
                aggregated[targetName].Quantity += quantity;
                aggregated[targetName].Total += subtotal;
            }
            else
            {
                aggregated[targetName] = new AggregatedProductSales
                {
                    ProductName = targetName,
                    Quantity = quantity,
                    Total = subtotal
                };
            }
        }

        return aggregated.Values.ToList();
    }

    private class AggregatedProductSales
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
