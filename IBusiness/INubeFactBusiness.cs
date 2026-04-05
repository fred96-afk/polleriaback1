using Models.Orders;

namespace IBusiness;

public interface INubeFactBusiness
{
    Task<bool> GenerateInvoiceAsync(int orderId);
}
