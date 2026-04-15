using IBusiness;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Models;

namespace Business;

public class EmailService(IOptions<EmailSettings> emailSettings) : IEmailService
{
    private readonly EmailSettings _settings = emailSettings.Value;

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendVerificationEmailAsync(string to, string name, string token)
    {
        var frontendBaseUrl = _settings.FrontendBaseUrl.TrimEnd('/');
        string verificationLink = $"{frontendBaseUrl}/verify-email?token={token}";
        
        string subject = "Verifica tu cuenta - Pollería";
        string body = $@"
            <h1>Hola {name},</h1>
            <p>Gracias por registrarte en nuestra Pollería. Para activar tu cuenta, por favor haz clic en el siguiente enlace:</p>
            <p><a href='{verificationLink}'>Verificar mi correo electrónico</a></p>
            <p>Si no creaste esta cuenta, puedes ignorar este mensaje.</p>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string to, string name, string token)
    {
        var frontendBaseUrl = _settings.FrontendBaseUrl.TrimEnd('/');
        string resetLink = $"{frontendBaseUrl}/reset-password?token={token}";

        string subject = "Restablece tu contraseña - Pollería";
        string body = $@"
            <h1>Hola {name},</h1>
            <p>Recibimos una solicitud para restablecer tu contraseña.</p>
            <p><a href='{resetLink}'>Restablecer mi contraseña</a></p>
            <p>Este enlace vence en 1 hora.</p>
            <p>Si no solicitaste este cambio, puedes ignorar este mensaje.</p>
        ";

        await SendEmailAsync(to, subject, body);
    }

    public async Task SendOrderInvoiceEmailAsync(string to, string name, string orderId, decimal total, string pdfUrl)
    {
        string subject = $"Comprobante de Pago - Pedido #{orderId}";
        string body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                <h2 style='color: #d32f2f; text-align: center;'>¡Gracias por tu compra!</h2>
                <p>Hola <strong>{name}</strong>,</p>
                <p>Tu pedido ha sido procesado con éxito. Aquí tienes un resumen:</p>
                <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <p><strong>Pedido ID:</strong> #{orderId}</p>
                    <p><strong>Total:</strong> S/ {total:F2}</p>
                </div>
                <p>Puedes descargar tu comprobante electrónico (Boleta) haciendo clic en el siguiente botón:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{pdfUrl}' style='background-color: #d32f2f; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Descargar PDF</a>
                </div>
                <hr style='border: 0; border-top: 1px solid #eee;'>
                <p style='font-size: 12px; color: #777; text-align: center;'>
                    Si tienes alguna duda, por favor contáctanos.<br>
                    <strong>Pollería Central</strong>
                </p>
            </div>
        ";

        await SendEmailAsync(to, subject, body);
    }
}
