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
        
        var clients = await clientRepository.GetAllAsync();
        var clientDict = clients.ToDictionary(c => c.Id, c => c.Name);

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
                        col.Item().Text("Reporte de Ventas").FontSize(14);
                        col.Item().Text($"Periodo: {request.StartDate:dd/MM/yyyy} - {request.EndDate:dd/MM/yyyy}").FontSize(10);
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text($"Fecha de Emisión: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
                });

                // Content
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); // #
                            columns.ConstantColumn(100); // Fecha
                            columns.RelativeColumn();   // Cliente
                            columns.ConstantColumn(80);  // Total
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("#");
                            header.Cell().Element(CellStyle).Text("Fecha");
                            header.Cell().Element(CellStyle).Text("Cliente");
                            header.Cell().Element(CellStyle).Text("Total");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold())
                                                .PaddingVertical(5)
                                                .BorderBottom(1)
                                                .BorderColor(Colors.Black);
                            }
                        });

                        int i = 1;
                        foreach (var order in orderList)
                        {
                            table.Cell().Element(CellStyle).Text(i++.ToString());
                            table.Cell().Element(CellStyle).Text(order.OrderDate.ToString("dd/MM/yyyy HH:mm"));
                            
                            string clientName = "General";
                            if (order.ClientId.HasValue && clientDict.TryGetValue(order.ClientId.Value, out var name))
                                clientName = name;

                            table.Cell().Element(CellStyle).Text(clientName);
                            table.Cell().Element(CellStyle).AlignRight().Text($"S/ {order.TotalAmount:F2}");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                            }
                        }
                    });

                    col.Item().PaddingTop(10).AlignRight().Column(summary =>
                    {
                        var totalAmount = orderList.Sum(o => o.TotalAmount);
                        summary.Item().Text($"Total de Pedidos: {orderList.Count}").FontSize(11);
                        summary.Item().Text($"Monto Total: S/ {totalAmount:F2}").FontSize(14).SemiBold().FontColor(Colors.Red.Medium);
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
        
        var clients = await clientRepository.GetAllAsync();
        var clientDict = clients.ToDictionary(c => c.Id, c => c.Name);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ventas");

        // Title
        var titleRange = worksheet.Range("A1:D1");
        titleRange.Merge().Value = "REPORTE DE VENTAS - POLLERÍA EL GIGANTE";
        titleRange.Style.Font.Bold = true;
        titleRange.Style.Font.FontSize = 16;
        titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Date Range
        worksheet.Cell("A2").Value = $"Periodo: {request.StartDate:dd/MM/yyyy} - {request.EndDate:dd/MM/yyyy}";
        worksheet.Range("A2:D2").Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Headers
        var headers = new[] { "ID", "Fecha", "Cliente", "Total (S/)" };
        for (int h = 0; h < headers.Length; h++)
        {
            var cell = worksheet.Cell(4, h + 1);
            cell.Value = headers[h];
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#D32F2F"); // Red Medium
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Font.Bold = true;
        }

        // Data
        int row = 5;
        foreach (var order in orderList)
        {
            worksheet.Cell(row, 1).Value = order.Id;
            worksheet.Cell(row, 2).Value = order.OrderDate;
            
            string clientName = "General";
            if (order.ClientId.HasValue && clientDict.TryGetValue(order.ClientId.Value, out var name))
                clientName = name;

            worksheet.Cell(row, 3).Value = clientName;
            worksheet.Cell(row, 4).Value = order.TotalAmount;
            row++;
        }

        // Summary
        int lastRow = row;
        worksheet.Cell(lastRow, 3).Value = "TOTAL:";
        worksheet.Cell(lastRow, 3).Style.Font.Bold = true;
        worksheet.Cell(lastRow, 4).FormulaA1 = $"=SUM(D5:D{lastRow - 1})";
        worksheet.Cell(lastRow, 4).Style.Font.Bold = true;
        worksheet.Cell(lastRow, 4).Style.NumberFormat.Format = "S/ #,##0.00";

        // Formatting
        worksheet.Columns().AdjustToContents();
        worksheet.Range(4, 1, lastRow, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        worksheet.Range(4, 1, lastRow, 4).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        worksheet.Column(2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
        worksheet.Column(4).Style.NumberFormat.Format = "S/ #,##0.00";

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
