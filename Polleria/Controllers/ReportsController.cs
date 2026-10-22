using IBusiness;
using Microsoft.AspNetCore.Mvc;
using Models.Reports;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController(
    IReportBusiness reportBusiness,
    IRabbitMQProducer rabbitMQProducer,
    IReportStorageService reportStorageService) : ControllerBase
{
    /// <summary>
    /// Encola la generación asíncrona de un reporte en formato Excel o PDF a través de RabbitMQ.
    /// Al finalizar, se emitirá una notificación push vía Pusher.
    /// </summary>
    [HttpPost("queue")]
    public async Task<IActionResult> QueueReport([FromBody] QueueReportRequest request)
    {
        if (request.StartDate > request.EndDate)
        {
            return BadRequest(new { message = "La fecha de inicio no puede ser posterior a la fecha de fin." });
        }

        var reportType = string.IsNullOrWhiteSpace(request.ReportType) ? "Excel" : request.ReportType;
        if (!reportType.Equals("Excel", StringComparison.OrdinalIgnoreCase) &&
            !reportType.Equals("Pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Tipo de reporte inválido. Valores aceptados: 'Excel' o 'Pdf'." });
        }

        var normalizedType = reportType.Equals("Excel", StringComparison.OrdinalIgnoreCase) ? "Excel" : "Pdf";
        var message = new ReportQueueMessage
        {
            JobId = Guid.NewGuid().ToString(),
            ReportType = normalizedType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Channel = string.IsNullOrWhiteSpace(request.Channel) ? "reports" : request.Channel,
            UserId = request.UserId,
            RequestedAt = DateTime.UtcNow
        };

        var published = await rabbitMQProducer.PublishReportRequestAsync(message);
        if (!published)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "No se pudo encolar la solicitud de reporte en RabbitMQ. Verifique el estado del broker."
            });
        }

        return Accepted(new QueueReportResponse
        {
            JobId = message.JobId,
            ReportType = message.ReportType,
            Status = "Queued",
            Channel = message.Channel,
            Message = $"Reporte {message.ReportType} encolado exitosamente. Recibirá una notificación push cuando esté listo.",
            RequestedAt = message.RequestedAt
        });
    }

    /// <summary>
    /// Encola específicamente la generación de un reporte de ventas en formato Excel.
    /// </summary>
    [HttpPost("sales/excel/queue")]
    public async Task<IActionResult> QueueSalesExcel([FromBody] QueueReportRequest? request, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string? channel)
    {
        var finalStartDate = request?.StartDate ?? startDate ?? DateTime.Today.AddDays(-30);
        var finalEndDate = request?.EndDate ?? endDate ?? DateTime.Today;
        var finalChannel = request?.Channel ?? channel ?? "reports";

        return await QueueReport(new QueueReportRequest
        {
            StartDate = finalStartDate,
            EndDate = finalEndDate,
            ReportType = "Excel",
            Channel = finalChannel,
            UserId = request?.UserId
        });
    }

    /// <summary>
    /// Encola específicamente la generación de un reporte de ventas en formato PDF.
    /// </summary>
    [HttpPost("sales/pdf/queue")]
    public async Task<IActionResult> QueueSalesPdf([FromBody] QueueReportRequest? request, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string? channel)
    {
        var finalStartDate = request?.StartDate ?? startDate ?? DateTime.Today.AddDays(-30);
        var finalEndDate = request?.EndDate ?? endDate ?? DateTime.Today;
        var finalChannel = request?.Channel ?? channel ?? "reports";

        return await QueueReport(new QueueReportRequest
        {
            StartDate = finalStartDate,
            EndDate = finalEndDate,
            ReportType = "Pdf",
            Channel = finalChannel,
            UserId = request?.UserId
        });
    }

    /// <summary>
    /// Descarga un reporte previamente generado por los workers de RabbitMQ.
    /// </summary>
    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> DownloadReport(string fileName)
    {
        var (fileBytes, contentType) = await reportStorageService.GetReportWithContentTypeAsync(fileName);
        if (fileBytes == null)
        {
            return NotFound(new { message = $"El archivo '{fileName}' no fue encontrado o aún no ha sido generado." });
        }

        return File(fileBytes, contentType, fileName);
    }

    /// <summary>
    /// Consulta el estado de generación de un reporte mediante su JobId.
    /// </summary>
    [HttpGet("status/{jobId}")]
    public IActionResult GetReportStatus(string jobId)
    {
        var storageDirectory = Path.Combine(AppContext.BaseDirectory, "reports_storage");
        if (Directory.Exists(storageDirectory))
        {
            var matchingFiles = Directory.GetFiles(storageDirectory, $"ReporteVentas_{jobId}_*.*");
            if (matchingFiles.Length > 0)
            {
                var file = new FileInfo(matchingFiles[0]);
                return Ok(new
                {
                    jobId,
                    status = "Completed",
                    fileName = file.Name,
                    downloadUrl = reportStorageService.GetDownloadUrl(file.Name),
                    fileSize = file.Length,
                    createdAt = file.CreationTimeUtc
                });
            }
        }

        return Ok(new
        {
            jobId,
            status = "ProcessingOrQueued",
            message = "El reporte se encuentra en cola o siendo procesado por los workers."
        });
    }

    /// <summary>
    /// Generación síncrona directa de reporte PDF (retrocompatibilidad).
    /// </summary>
    [HttpGet("sales/pdf")]
    public async Task<IActionResult> GetSalesPdf([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var request = new SalesReportRequest(startDate, endDate);
        var pdf = await reportBusiness.GenerateSalesPdfAsync(request);
        return File(pdf, "application/pdf", $"ReporteVentas_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
    }

    /// <summary>
    /// Generación síncrona directa de reporte Excel (retrocompatibilidad).
    /// </summary>
    [HttpGet("sales/excel")]
    public async Task<IActionResult> GetSalesExcel([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var request = new SalesReportRequest(startDate, endDate);
        var excel = await reportBusiness.GenerateSalesExcelAsync(request);
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ReporteVentas_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx");
    }
}
