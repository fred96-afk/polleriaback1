namespace IBusiness;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendVerificationEmailAsync(string to, string name, string token);
    Task SendPasswordResetEmailAsync(string to, string name, string token);
    Task SendOrderInvoiceEmailAsync(string to, string name, string orderId, decimal total, string pdfUrl);
}
