using DbModel;
using ClosedXML.Excel;
using IBusiness;
using IRepository;
using Models.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Business;

public class ReportBusiness(IOrderRepository orderRepository, IClientRepository clientRepository) : IReportBusiness
{
    static ReportBusiness()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateSalesPdfAsync(SalesReportRequest request)
    {
        var orders = await orderRepository.GetByDateRangeAsync(request.StartDate, request.EndDate);
        var orderList = orders.ToList();

        // Agrupar ventas por producto
        var productSales = orderList
            .SelectMany(o => o.OrderDetails)
            .GroupBy(d => new { d.ProductId, ProductName = d.Product?.Name ?? $"Producto {d.ProductId}" })
            .Select(g => new
            {
                g.Key.ProductName,
                Quantity = g.Sum(d => d.Quantity),
                Total = g.Sum(d => d.Subtotal)
            })
            .OrderByDescending(x => x.Quantity)
            .ToList();

        var totalAmount = orderList.Sum(o => o.TotalAmount);
        var mostSold = productSales.FirstOrDefault();
        var leastSold = productSales.LastOrDefault();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                // Header
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("POLLERÍA EL GIGANTE").FontSize(20).SemiBold().FontColor(Colors.Red.Medium);
                        col.Item().Text("Reporte Estadístico de Ventas por Producto").FontSize(14);
                        col.Item().Text($"Periodo: {request.StartDate:dd/MM/yyyy} - {request.EndDate:dd/MM/yyyy}").FontSize(10);
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text($"Fecha de Emisión: {PeruTimeHelper.Now:dd/MM/yyyy HH:mm}");
                    });
                });

                // Content
                page.Content().PaddingVertical(10).Column(col =>
                {
                    // Resumen Estadístico
                    col.Item().PaddingBottom(10).Row(row =>
                    {
                        row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Column(c =>
                        {
                            c.Item().Text("PRODUCTO MÁS VENDIDO").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                            c.Item().Text(mostSold?.ProductName ?? "N/A").FontSize(12).SemiBold();
                            c.Item().Text($"{mostSold?.Quantity ?? 0} unidades").FontSize(10);
                        });
                        row.ConstantItem(10);
                        row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Column(c =>
                        {
                            c.Item().Text("PRODUCTO MENOS VENDIDO").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                            c.Item().Text(leastSold?.ProductName ?? "N/A").FontSize(12).SemiBold();
                            c.Item().Text($"{leastSold?.Quantity ?? 0} unidades").FontSize(10);
                        });
                        row.ConstantItem(10);
                        row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Background(Colors.Red.Lighten5).Column(c =>
                        {
                            c.Item().Text("TOTAL RECAUDADO").FontSize(8).SemiBold().FontColor(Colors.Red.Medium);
                            c.Item().Text($"S/ {totalAmount:F2}").FontSize(14).SemiBold().FontColor(Colors.Red.Medium);
                            c.Item().Text($"{orderList.Count} pedidos").FontSize(10);
                        });
                    });

                    col.Item().PaddingBottom(5).Text("Detalle de Ventas por Producto").FontSize(12).SemiBold();

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); // #
                            columns.RelativeColumn();   // Producto
                            columns.ConstantColumn(80);  // Cantidad
                            columns.ConstantColumn(100); // Total S/
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("#");
                            header.Cell().Element(CellStyle).Text("Producto");
                            header.Cell().Element(CellStyle).AlignRight().Text("Cant.");
                            header.Cell().Element(CellStyle).AlignRight().Text("Total S/");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold())
                                                .PaddingVertical(5)
                                                .BorderBottom(1)
                                                .BorderColor(Colors.Black);
                            }
                        });

                        int i = 1;
                        if (!productSales.Any())
                        {
                            table.Cell().ColumnSpan(4).PaddingVertical(20).AlignCenter().Text("No se encontraron ventas en este periodo.").Italic();
                        }
                        
                        foreach (var item in productSales)
                        {
                            table.Cell().Element(CellStyle).Text(i++.ToString());
                            table.Cell().Element(CellStyle).Text(item.ProductName);
                            table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                            table.Cell().Element(CellStyle).AlignRight().Text($"S/ {item.Total:F2}");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                            }
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateSalesExcelAsync(SalesReportRequest request)
    {
        var orders = await orderRepository.GetByDateRangeAsync(request.StartDate, request.EndDate);
        var orderList = orders.ToList();

        var productSales = orderList
            .SelectMany(o => o.OrderDetails)
            .GroupBy(d => new { d.ProductId, ProductName = d.Product?.Name ?? $"Producto {d.ProductId}" })
            .Select(g => new
            {
                g.Key.ProductName,
                Quantity = g.Sum(d => d.Quantity),
                Total = g.Sum(d => d.Subtotal)
            })
            .OrderByDescending(x => x.Quantity)
            .ToList();

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

        // Headers
        var headers = new[] { "Producto", "Cantidad Vendida", "Total Recaudado (S/)" };
        for (int h = 0; h < headers.Length; h++)
        {
            var cell = worksheet.Cell(4, h + 1);
            cell.Value = headers[h];
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#D32F2F");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Font.Bold = true;
        }

        // Data
        int row = 5;
        foreach (var item in productSales)
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
        worksheet.Cell(lastRow, 3).FormulaA1 = $"=SUM(C5:C{lastRow - 1})";
        worksheet.Cell(lastRow, 3).Style.Font.Bold = true;
        worksheet.Cell(lastRow, 3).Style.NumberFormat.Format = "S/ #,##0.00";

        // Formatting
        worksheet.Columns().AdjustToContents();
        worksheet.Range(4, 1, lastRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        worksheet.Range(4, 1, lastRow, 3).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        worksheet.Column(3).Style.NumberFormat.Format = "S/ #,##0.00";

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
