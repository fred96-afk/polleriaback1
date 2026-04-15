using IBusiness;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Error;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Configuration;
using Models;
using Models.Orders;

namespace Business;

public class MercadoPagoBusiness : IMercadoPagoBusiness
{
    private readonly MercadoPagoSettings _settings;

    public MercadoPagoBusiness(IConfiguration configuration)
    {
        _settings = configuration.GetSection("MercadoPago").Get<MercadoPagoSettings>()
            ?? throw new InvalidOperationException("MercadoPago settings not found.");

        if (string.IsNullOrWhiteSpace(_settings.AccessToken))
        {
            throw new InvalidOperationException("MercadoPago AccessToken not found.");
        }

        MercadoPagoConfig.AccessToken = _settings.AccessToken;
    }

    public async Task<string> CreatePaymentPreferenceAsync(OrderResponse order, string payerEmail)
    {
        if (order.Details.Count == 0)
        {
            throw new InvalidOperationException("No se puede generar una preferencia de pago sin productos.");
        }

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

        var frontendBaseUrl = (_settings.FrontendBaseUrl ?? string.Empty).TrimEnd('/');
        if (string.IsNullOrWhiteSpace(frontendBaseUrl))
        {
            throw new InvalidOperationException("MercadoPago FrontendBaseUrl not found.");
        }

        var successUrl = $"{frontendBaseUrl}/checkout/success";
        var request = new PreferenceRequest
        {
            Items = items,
            ExternalReference = order.Id.ToString(),
            StatementDescriptor = BuildStatementDescriptor(_settings.StatementDescriptor),
            Payer = new PreferencePayerRequest
            {
                Email = payerEmail
            },
            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = successUrl,
                Failure = $"{frontendBaseUrl}/checkout/failure",
                Pending = $"{frontendBaseUrl}/checkout/pending"
            },
            AutoReturn = CanUseAutoReturn(successUrl) ? "approved" : null,
            BinaryMode = true,
            PaymentMethods = new PreferencePaymentMethodsRequest
            {
                Installments = 1
            },
            NotificationUrl = string.IsNullOrWhiteSpace(_settings.NotificationUrl) ? null : _settings.NotificationUrl
        };

        try
        {
            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request);

            var paymentUrl = _settings.UseSandbox ? preference.SandboxInitPoint : preference.InitPoint;
            if (string.IsNullOrWhiteSpace(paymentUrl))
            {
                throw new InvalidOperationException("Mercado Pago no devolvió una URL de pago válida.");
            }

            return paymentUrl;
        }
        catch (MercadoPagoApiException ex)
        {
            var apiMessage = ex.ApiResponse?.Content ?? ex.Message;
            throw new InvalidOperationException($"Mercado Pago rechazó la preferencia: {apiMessage}", ex);
        }
        catch (MercadoPagoException ex)
        {
            throw new InvalidOperationException($"Error de comunicación con Mercado Pago: {ex.Message}", ex);
        }
    }

    private static string BuildStatementDescriptor(string? descriptor)
    {
        var value = string.IsNullOrWhiteSpace(descriptor) ? "POLLERIA" : descriptor.Trim().ToUpperInvariant();
        return value.Length <= 13 ? value : value[..13];
    }

    private static bool CanUseAutoReturn(string successUrl)
    {
        if (!Uri.TryCreate(successUrl, UriKind.Absolute, out var uri))
        {
            return false;
        }

        // Mercado Pago solo permite auto_return en producción si la URL es HTTPS
        return string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) && !uri.IsLoopback;
    }
}
