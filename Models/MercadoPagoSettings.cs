namespace Models;

public class MercadoPagoSettings
{
    public string AccessToken { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string FrontendBaseUrl { get; set; } = "http://localhost:4200";
    public string? NotificationUrl { get; set; }
    public string StatementDescriptor { get; set; } = "POLLERIA";
    public bool UseSandbox { get; set; }
}
