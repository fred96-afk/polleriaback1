using System.Text.Json;
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
    public async Task<IActionResult> Webhook([FromQuery] string? topic, [FromQuery] string? id, [FromBody] JsonElement? body)
    {
        string? finalTopic = topic;
        string? finalId = id;

        // Si no vienen por query, intentamos extraer del body (Webhook v2)
        if (body.HasValue && body.Value.ValueKind != JsonValueKind.Null)
        {
            if (string.IsNullOrEmpty(finalTopic) && body.Value.TryGetProperty("type", out var typeProp))
            {
                finalTopic = typeProp.GetString();
            }
            if (string.IsNullOrEmpty(finalId) && body.Value.TryGetProperty("data", out var dataProp) && dataProp.TryGetProperty("id", out var idProp))
            {
                finalId = idProp.GetString();
            }
        }

        Console.WriteLine($"[MercadoPago Webhook] Topic: {finalTopic}, Id: {finalId}");

        if (finalTopic != "payment" || string.IsNullOrEmpty(finalId))
        {
            return Ok();
        }

        await ProcessPaymentAsync(finalId);
        return Ok();
    }

    [HttpGet("verify/{paymentId}")]
    public async Task<IActionResult> Verify(string paymentId)
    {
        Console.WriteLine($"[MercadoPago Verify] Verificando pago: {paymentId}");
        var result = await ProcessPaymentAsync(paymentId);
        
        if (result) return Ok(new { message = "Estado de pago actualizado correctamente." });
        return BadRequest(new { message = "No se pudo actualizar el estado del pago. Verifique el ID o la referencia externa." });
    }

    private async Task<bool> ProcessPaymentAsync(string paymentId)
    {
        try
        {
            var client = new PaymentClient();
            var payment = await client.GetAsync(long.Parse(paymentId));

            if (payment == null)
            {
                Console.WriteLine($"[MercadoPago] Pago {paymentId} no encontrado.");
                return false;
            }

            Console.WriteLine($"[MercadoPago] Pago {paymentId} recuperado. Status: {payment.Status}, ExternalRef: {payment.ExternalReference}");

            if (int.TryParse(payment.ExternalReference, out int orderId))
            {
                if (payment.Status == "approved")
                {
                    Console.WriteLine($"[MercadoPago] Aprobando pedido {orderId}");
                    await _orderBusiness.UpdatePaymentStatusAsync(orderId, "Approved");
                    await _orderBusiness.UpdateStatusAsync(orderId, "Accepted");
                    return true;
                }
                else if (payment.Status == "rejected")
                {
                    Console.WriteLine($"[MercadoPago] Rechazando pago de pedido {orderId}");
                    await _orderBusiness.UpdatePaymentStatusAsync(orderId, "Rejected");
                    return true;
                }
                else if (payment.Status == "cancelled")
                {
                    Console.WriteLine($"[MercadoPago] Pago cancelado para pedido {orderId}");
                    await _orderBusiness.UpdatePaymentStatusAsync(orderId, "Cancelled");
                    return true;
                }
            }
            else
            {
                Console.WriteLine($"[MercadoPago] No se pudo parsear ExternalReference: {payment.ExternalReference}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MercadoPago] Error al procesar pago {paymentId}: {ex.Message}");
        }
        return false;
    }
}
