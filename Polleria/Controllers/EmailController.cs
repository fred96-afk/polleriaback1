using System.Net.Mail;
using IBusiness;
using IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Email;
using Models.Reports;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmailController(
    IUserRepository userRepository,
    IOrderRepository orderRepository,
    IInvoiceRepository invoiceRepository,
    IEmailService emailService,
    IReportBusiness reportBusiness,
    IReportStorageService reportStorageService,
    ILogger<EmailController> logger) : ControllerBase
{
    [HttpPost("test-templates")]
    public async Task<IActionResult> SendTemplateTests([FromBody] EmailTemplateTestRequest request)
    {
        if (!MailAddress.TryCreate(request.Email, out var address))
        {
            return BadRequest(new { message = "La dirección de correo no es válida." });
        }

        var users = await userRepository.GetAllAsync();
        var sampleUser = users.FirstOrDefault(user => user.IsVerified);
        if (sampleUser == null)
        {
            return Conflict(new { message = "No hay usuarios verificados en la base de datos para usar como datos de muestra." });
        }

        var sampleOrder = (await orderRepository.GetAllWithIncludesAsync()).FirstOrDefault();
        if (sampleOrder == null)
        {
            return Conflict(new { message = "No hay pedidos en la base de datos para usar como datos de muestra." });
        }

        var invoices = await invoiceRepository.FindAsync(invoice => invoice.OrderId == sampleOrder.Id);
        var invoiceUrl = invoices.FirstOrDefault()?.PdfUrl;
        var orderName = sampleOrder.Client?.Name ?? sampleUser.Name;
        var orderDate = sampleOrder.OrderDate;
        var startDate = orderDate.Date;
        var endDate = orderDate.Date;
        var reportBytes = await reportBusiness.GenerateSalesPdfAsync(new SalesReportRequest(startDate, endDate));
        var reportFileName = $"EmailTemplateTest_{Guid.NewGuid():N}.pdf";
        await reportStorageService.SaveReportAsync(reportFileName, reportBytes);
        var reportUrl = $"{Request.Scheme}://{Request.Host}{reportStorageService.GetDownloadUrl(reportFileName)}";
        var testToken = $"test-{Guid.NewGuid():N}";

        var sends = new (string Name, Func<Task> Send)[]
        {
            ("creacion-de-cuenta", () => emailService.SendVerificationEmailAsync(address.Address, sampleUser.Name, testToken)),
            ("recuperacion-de-cuenta", () => emailService.SendPasswordResetEmailAsync(address.Address, sampleUser.Name, testToken)),
            ("compra", () => emailService.SendOrderInvoiceEmailAsync(
                address.Address,
                orderName,
                sampleOrder.Id.ToString(),
                sampleOrder.TotalAmount,
                invoiceUrl,
                "Datos de prueba",
                sampleOrder.Type.ToString(),
                orderDate)),
            ("reporte-generado", () => emailService.SendReportReadyEmailAsync(
                address.Address,
                sampleUser.Name,
                $"Reporte de ventas de prueba (pedido #{sampleOrder.Id})",
                "PDF",
                reportUrl,
                startDate,
                endDate,
                DateTime.UtcNow)),
            ("mantenimiento", () => emailService.SendMaintenanceNoticeEmailAsync(
                address.Address,
                sampleUser.Name,
                request.ScheduledDate,
                request.EstimatedDuration,
                request.Details,
                request.SupportContact))
        };

        var results = new List<EmailTemplateSendResult>(sends.Length);
        foreach (var send in sends)
        {
            try
            {
                await send.Send();
                results.Add(new EmailTemplateSendResult(send.Name, true));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falló el correo de prueba {Template} para {Email}.", send.Name, address.Address);
                results.Add(new EmailTemplateSendResult(send.Name, false));
            }
        }

        var sentCount = results.Count(result => result.Sent);
        var response = new
        {
            recipient = address.Address,
            sent = sentCount,
            failed = sends.Length - sentCount,
            results,
            note = "Los enlaces de verificación y recuperación son de prueba y no son válidos."
        };

        return sentCount == 0
            ? StatusCode(StatusCodes.Status502BadGateway, response)
            : Ok(response);
    }

    private sealed record EmailTemplateSendResult(string Template, bool Sent);
}
