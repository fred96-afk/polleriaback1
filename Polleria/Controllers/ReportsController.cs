using IBusiness;
using Microsoft.AspNetCore.Mvc;
using Models.Reports;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(IReportBusiness reportBusiness) : ControllerBase
{
    [HttpGet("sales/pdf")]
    public async Task<IActionResult> GetSalesPdf([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var request = new SalesReportRequest(startDate, endDate);
        var pdf = await reportBusiness.GenerateSalesPdfAsync(request);
        return File(pdf, "application/pdf", $"ReporteVentas_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
    }

    [HttpGet("sales/excel")]
    public async Task<IActionResult> GetSalesExcel([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var request = new SalesReportRequest(startDate, endDate);
        var excel = await reportBusiness.GenerateSalesExcelAsync(request);
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ReporteVentas_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx");
    }
}
