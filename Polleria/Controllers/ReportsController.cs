using System.Security.Claims;
using IBusiness;
using IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Reports;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportsController(
    IReportBusiness reportBusiness,
    IRabbitMQProducer rabbitMQProducer,
    IReportStorageService reportStorageService,
    IUserRepository userRepository) : ControllerBase
{
    /// <summary>
    /// Encola la generación asíncrona de un reporte en formato Excel o PDF a través de RabbitMQ.
    /// Al finalizar, se emitirá una notificación push vía Pusher.
    /// </summary>
    [HttpPost("queue")]
    public async Task<IActionResult> QueueReport([FromBody] QueueReportRequest request)
    {
        var finalStartDate = request.StartDate.HasValue && request.StartDate.Value != default
            ? request.StartDate.Value
            : DateTime.Today.AddDays(-30);

        var finalEndDate = request.EndDate.HasValue && request.EndDate.Value != default
            ? request.EndDate.Value
            : DateTime.Today;

        if (finalStartDate > finalEndDate)
        {
            return BadRequest(new { message = "La fecha de inicio no puede ser posterior a la fecha de fin." });
        }

        var reportType = string.IsNullOrWhiteSpace(request.ReportType) ? "Excel" : request.ReportType;
        if (!reportType.Equals("Excel", StringComparison.OrdinalIgnoreCase) &&
            !reportType.Equals("Pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Tipo de reporte inválido. Valores aceptados: 'Excel' o 'Pdf'." });
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized();
        }

        var normalizedType = reportType.Equals("Excel", StringComparison.OrdinalIgnoreCase) ? "Excel" : "Pdf";
        var message = new ReportQueueMessage
        {
            JobId = Guid.NewGuid().ToString(),
            ReportType = normalizedType,
            StartDate = finalStartDate,
            EndDate = finalEndDate,
            Channel = string.IsNullOrWhiteSpace(request.Channel) ? "reports" : request.Channel,
            UserId = user.Id.ToString(),
            UserEmail = user.Email,
            UserName = user.Name,
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
        var finalStartDate = (request?.StartDate.HasValue == true && request.StartDate.Value != default)
            ? request.StartDate.Value
            : (startDate.HasValue && startDate.Value != default ? startDate.Value : DateTime.Today.AddDays(-30));

        var finalEndDate = (request?.EndDate.HasValue == true && request.EndDate.Value != default)
            ? request.EndDate.Value
            : (endDate.HasValue && endDate.Value != default ? endDate.Value : DateTime.Today);

        var finalChannel = !string.IsNullOrWhiteSpace(channel) 
            ? channel 
            : (!string.IsNullOrWhiteSpace(request?.Channel) ? request.Channel : "reports");

        return await QueueReport(new QueueReportRequest
        {
            StartDate = finalStartDate,
            EndDate = finalEndDate,
            ReportType = "Excel",
            Channel = finalChannel
        });
    }

    /// <summary>
    /// Encola específicamente la generación de un reporte de ventas en formato PDF.
    /// </summary>
    [HttpPost("sales/pdf/queue")]
    public async Task<IActionResult> QueueSalesPdf([FromBody] QueueReportRequest? request, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string? channel)
    {
        var finalStartDate = (request?.StartDate.HasValue == true && request.StartDate.Value != default)
            ? request.StartDate.Value
            : (startDate.HasValue && startDate.Value != default ? startDate.Value : DateTime.Today.AddDays(-30));

        var finalEndDate = (request?.EndDate.HasValue == true && request.EndDate.Value != default)
            ? request.EndDate.Value
            : (endDate.HasValue && endDate.Value != default ? endDate.Value : DateTime.Today);

        var finalChannel = !string.IsNullOrWhiteSpace(channel) 
            ? channel 
            : (!string.IsNullOrWhiteSpace(request?.Channel) ? request.Channel : "reports");

        return await QueueReport(new QueueReportRequest
        {
            StartDate = finalStartDate,
            EndDate = finalEndDate,
            ReportType = "Pdf",
            Channel = finalChannel
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
                var extension = file.Extension.ToLowerInvariant();
                var reportType = extension == ".xlsx" ? "Excel" : "Pdf";

                return Ok(new
                {
                    jobId,
                    status = "Completed",
                    reportType,
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
    /// Obtiene el historial de reportes completados disponibles para descarga.
    /// </summary>
    [HttpGet("history")]
    public async Task<IActionResult> GetReportsHistory()
    {
        var reports = await reportStorageService.GetCompletedReportsAsync();
        return Ok(reports);
    }

    /// <summary>
    /// Elimina un reporte generado del almacenamiento en disco.
    /// </summary>
    [HttpDelete("{fileName}")]
    public async Task<IActionResult> DeleteReport(string fileName)
    {
        var deleted = await reportStorageService.DeleteReportAsync(fileName);
        if (!deleted)
        {
            return NotFound(new { message = $"El archivo '{fileName}' no fue encontrado." });
        }
        return Ok(new { message = $"Reporte '{fileName}' eliminado exitosamente." });
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
