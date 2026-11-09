namespace IBusiness;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendVerificationEmailAsync(string to, string name, string token);
    Task SendPasswordResetEmailAsync(string to, string name, string token);
    Task SendOrderInvoiceEmailAsync(
        string to,
        string name,
        string orderId,
        decimal total,
        string? pdfUrl = null,
        string? paymentMethod = null,
        string? orderType = null,
        DateTime? orderDate = null,
        string? trackingUrl = null);
    Task SendReportReadyEmailAsync(
        string to,
        string adminName,
        string reportTitle,
        string reportType,
        string downloadUrl,
        DateTime? startDate = null,
        DateTime? endDate = null,
        DateTime? generatedAt = null);
    Task SendMaintenanceNoticeEmailAsync(
        string to,
        string recipientName,
        string scheduledDate,
        string estimatedDuration,
        string details,
        string? supportContact = null);
}
