using IBusiness;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Orders;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IOrderBusiness _orderBusiness;
    private readonly string _accessToken;

    public PaymentsController(IOrderBusiness orderBusiness, IConfiguration configuration)
    {
        _orderBusiness = orderBusiness;
        var settings = configuration.GetSection("MercadoPago");
        _accessToken = settings["AccessToken"] ?? string.Empty;
        MercadoPagoConfig.AccessToken = _accessToken;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromQuery] string topic, [FromQuery] string id)
    {
        // MP envía notificaciones por IPN o Webhooks. "topic" puede ser "payment" o "merchant_order"
        // Para pagos simplificados, nos interesa topic="payment"
        if (topic != "payment")
        {
            return Ok(); // Respondemos 200 para que MP no reintente otros eventos que no manejamos
        }

        try
        {
            var client = new PaymentClient();
            var payment = await client.GetAsync(long.Parse(id));

            if (payment == null) return NotFound();

            // El "ExternalReference" que enviamos es el OrderId
            if (int.TryParse(payment.ExternalReference, out int orderId))
            {
                if (payment.Status == "approved")
                {
                    // Actualizamos estados
                    await _orderBusiness.UpdatePaymentStatusAsync(orderId, "Approved");
                    await _orderBusiness.UpdateStatusAsync(orderId, "Accepted");
                }
                else if (payment.Status == "rejected")
                {
                    await _orderBusiness.UpdatePaymentStatusAsync(orderId, "Rejected");
                }
            }

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en Webhook de Mercado Pago: {ex.Message}");
            return StatusCode(500);
        }
    }
}
