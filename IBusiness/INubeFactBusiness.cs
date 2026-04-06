namespace IBusiness;

public record NubeFactResult(bool Success, string? PdfUrl = null, string? Error = null);

public interface INubeFactBusiness
{
    Task<NubeFactResult> GenerateInvoiceAsync(int orderId);
}