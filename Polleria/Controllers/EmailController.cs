using IBusiness;
using IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Models;
using Models.Email;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
[EnableRateLimiting("EmailTemplateTests")]
public class EmailController(
    IProductRepository productRepository,
    IEmailService emailService,
    IOptions<EmailSettings> emailSettings,
    ILogger<EmailController> logger) : ControllerBase
{
    [HttpPost("test-templates")]
    public async Task<IActionResult> SendTemplateTests([FromBody] EmailTemplateTestRequest request)
    {
        if (!emailSettings.Value.EnablePublicTemplateTest)
        {
            return NotFound();
        }

        var recipient = emailSettings.Value.TemplateTestRecipient;
        if (string.IsNullOrWhiteSpace(recipient))
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                message = "El destinatario de prueba no está configurado en el servidor."
            });
        }

        var products = await productRepository.SearchAsync(string.Empty);
        var sampleProduct = products.OrderBy(product => product.Name).FirstOrDefault();
        if (sampleProduct == null)
        {
            return Conflict(new { message = "No hay productos en la base de datos para usar como datos de muestra." });
        }

        const string sampleName = "Cliente de prueba";
        const string sampleOrderId = "PRUEBA-001";
        var samplePrice = sampleProduct.IsOnSale
            ? sampleProduct.SalePrice!.Value
            : sampleProduct.BasePrice;
        var sampleDate = DateTime.UtcNow;
        var testToken = $"test-{Guid.NewGuid():N}";

        var sends = new (string Name, Func<Task> Send)[]
        {
            ("creacion-de-cuenta", () => emailService.SendVerificationEmailAsync(recipient, sampleName, testToken)),
            ("recuperacion-de-cuenta", () => emailService.SendPasswordResetEmailAsync(recipient, sampleName, testToken)),
            ("compra", () => emailService.SendOrderInvoiceEmailAsync(
                recipient,
                sampleName,
                sampleOrderId,
                samplePrice,
                paymentMethod: "Datos de prueba",
                orderType: "Pedido de prueba",
                orderDate: sampleDate)),
            ("reporte-generado", () => emailService.SendReportReadyEmailAsync(
                recipient,
                "Administrador de prueba",
                $"Reporte de prueba - {sampleProduct.Name}",
                "PDF",
                "https://example.com/reporte-de-prueba.pdf",
                sampleDate.Date,
                sampleDate.Date,
                sampleDate)),
            ("mantenimiento", () => emailService.SendMaintenanceNoticeEmailAsync(
                recipient,
                sampleName,
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
                logger.LogError(ex, "Falló el correo de prueba {Template}.", send.Name);
                results.Add(new EmailTemplateSendResult(send.Name, false));
            }
        }

        var sentCount = results.Count(result => result.Sent);
        var response = new
        {
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
