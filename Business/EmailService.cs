using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Business.Templates;
using IBusiness;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;

namespace Business;

public class EmailService(
    IOptions<EmailSettings> emailSettings,
    HttpClient httpClient,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailSettings _settings = emailSettings.Value;

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(_settings.ResendApiKey))
        {
            throw new InvalidOperationException("No se ha configurado EmailSettings:ResendApiKey.");
        }

        var senderName = string.IsNullOrWhiteSpace(_settings.SenderName) ? "Pollería El Gigante" : _settings.SenderName;
        var senderEmail = string.IsNullOrWhiteSpace(_settings.SenderEmail) ? "notificaciones@elrancho.space" : _settings.SenderEmail;

        var emailData = new
        {
            from = $"{senderName} <{senderEmail}>",
            to = new[] { to },
            subject = subject,
            html = body
        };

        var json = JsonSerializer.Serialize(emailData);
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ResendApiKey);

        try
        {
            var response = await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                logger.LogError("Error al enviar correo vía Resend a '{To}'. Estado: {StatusCode}. Respuesta: {Error}", to, (int)response.StatusCode, error);
                throw new InvalidOperationException($"Error enviando correo vía Resend: {error}");
            }

            logger.LogInformation("Correo enviado exitosamente a '{To}' con asunto '{Subject}'.", to, subject);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            logger.LogError(ex, "Fallo de conexión al enviar correo a '{To}'.", to);
            throw;
        }
    }

    /// <summary>
    /// 1. Plantilla: Creación de cuenta y verificación de email
    /// </summary>
    public async Task SendVerificationEmailAsync(string to, string name, string token)
    {
        var frontendBaseUrl = (_settings.FrontendBaseUrl ?? "https://polleriafront1.vercel.app").TrimEnd('/');
        var verificationLink = $"{frontendBaseUrl}/verify-email?token={Uri.EscapeDataString(token)}";
        var subject = "¡Bienvenido a Pollería El Gigante! Confirma tu cuenta";

        var body = EmailTemplates.AccountVerification(name, verificationLink, frontendBaseUrl);
        await SendEmailAsync(to, subject, body);
    }

    /// <summary>
    /// 2. Plantilla: Recuperación de cuenta / Restablecer contraseña
    /// </summary>
    public async Task SendPasswordResetEmailAsync(string to, string name, string token)
    {
        var frontendBaseUrl = (_settings.FrontendBaseUrl ?? "https://polleriafront1.vercel.app").TrimEnd('/');
        var resetLink = $"{frontendBaseUrl}/reset-password?token={Uri.EscapeDataString(token)}";
        var subject = "Restablece tu contraseña - Pollería El Gigante";

        var body = EmailTemplates.PasswordReset(name, resetLink, expirationHours: 1);
        await SendEmailAsync(to, subject, body);
    }

    /// <summary>
    /// 3. Plantilla: Confirmación de compra / Factura o Boleta electrónica
    /// </summary>
    public async Task SendOrderInvoiceEmailAsync(
        string to,
        string name,
        string orderId,
        decimal total,
        string? pdfUrl = null,
        string? paymentMethod = null,
        string? orderType = null,
        DateTime? orderDate = null,
        string? trackingUrl = null)
    {
        var frontendBaseUrl = (_settings.FrontendBaseUrl ?? "https://polleriafront1.vercel.app").TrimEnd('/');
        var defaultTracking = trackingUrl ?? $"{frontendBaseUrl}/orders/{orderId}";
        var subject = $"¡Pedido Confirmado #{orderId}! Gracias por tu compra - Pollería El Gigante";

        var body = EmailTemplates.OrderConfirmation(
            customerName: name,
            orderId: orderId,
            total: total,
            pdfUrl: pdfUrl,
            paymentMethod: paymentMethod,
            orderType: orderType,
            orderDate: orderDate ?? DateTime.UtcNow,
            trackingUrl: defaultTracking);

        await SendEmailAsync(to, subject, body);
    }

    /// <summary>
    /// 4. Plantilla: Notificación de reporte generado para el administrador
    /// </summary>
    public async Task SendReportReadyEmailAsync(
        string to,
        string adminName,
        string reportTitle,
        string reportType,
        string downloadUrl,
        DateTime? startDate = null,
        DateTime? endDate = null,
        DateTime? generatedAt = null)
    {
        var subject = $"Reporte Listo para Descarga: {reportTitle} - Pollería El Gigante";
        var body = EmailTemplates.AdminReportCompleted(
            adminName: adminName,
            reportTitle: reportTitle,
            reportType: reportType,
            downloadUrl: downloadUrl,
            startDate: startDate,
            endDate: endDate,
            generatedAt: generatedAt ?? DateTime.UtcNow);

        await SendEmailAsync(to, subject, body);
    }

    /// <summary>
    /// 5. Plantilla: Notificación de mantenimiento programado de la web / plataforma
    /// </summary>
    public async Task SendMaintenanceNoticeEmailAsync(
        string to,
        string recipientName,
        string scheduledDate,
        string estimatedDuration,
        string details,
        string? supportContact = null)
    {
        var subject = "Aviso Importante: Mantenimiento Programado de Plataforma - Pollería El Gigante";
        var body = EmailTemplates.MaintenanceNotice(
            recipientName: recipientName,
            scheduledDate: scheduledDate,
            estimatedDuration: estimatedDuration,
            details: details,
            supportContact: supportContact);

        await SendEmailAsync(to, subject, body);
    }
}