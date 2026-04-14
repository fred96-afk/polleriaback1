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
}
