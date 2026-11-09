namespace Models;

public class EmailSettings
{
    public string ResendApiKey { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string FrontendBaseUrl { get; set; } = "http://localhost:4200";
    public string TemplateTestRecipient { get; set; } = string.Empty;
    public bool EnablePublicTemplateTest { get; set; }
}
