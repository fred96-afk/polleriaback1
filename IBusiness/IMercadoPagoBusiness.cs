using Models.Orders;

namespace IBusiness;

public interface IMercadoPagoBusiness
{
    Task<string> CreatePaymentPreferenceAsync(OrderResponse order);
}
