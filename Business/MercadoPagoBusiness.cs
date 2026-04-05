using IBusiness;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Configuration;
using Models.Orders;

namespace Business;

public class MercadoPagoBusiness : IMercadoPagoBusiness
{
    private readonly string _accessToken;

    public MercadoPagoBusiness(IConfiguration configuration)
    {
        _accessToken = configuration["MercadoPago:AccessToken"] ?? throw new InvalidOperationException("MercadoPago AccessToken not found.");
        MercadoPagoConfig.AccessToken = _accessToken;
    }

    public async Task<string> CreatePaymentPreferenceAsync(OrderResponse order)
    {
        var items = order.Details.Select(d =>
        {
            var title = d.ProductName ?? $"Producto {d.ProductId}";
            if (!string.IsNullOrEmpty(d.SideName))
            {
                title += $" (+ {d.SideName})";
            }

            return new PreferenceItemRequest
            {
                Id = d.ProductId.ToString(),
                Title = title,
                Quantity = d.Quantity,
                CurrencyId = "PEN",
                UnitPrice = d.UnitPrice
            };
        }).ToList();

        var request = new PreferenceRequest
        {
            Items = items,
            ExternalReference = order.Id.ToString(),
            StatementDescriptor = "POLLERIA",
            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = "http://localhost:4200/checkout/success",
                Failure = "http://localhost:4200/checkout/failure",
                Pending = "http://localhost:4200/checkout/pending"
            },
            AutoReturn = "approved",
            BinaryMode = true,
            NotificationUrl = "https://tu-api.com/api/payments/webhook",
            PaymentMethods = new PreferencePaymentMethodsRequest
            {
                Installments = 1
            }
        };

        var client = new PreferenceClient();
        Preference preference = await client.CreateAsync(request);

        return preference.InitPoint;
    }
}
