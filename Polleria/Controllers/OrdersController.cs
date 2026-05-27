using IBusiness;
using Microsoft.AspNetCore.Mvc;
using Models.Orders;

namespace Polleria.Controllers;

[ApiController]
[Route("api/Pedidos")]
public class OrdersController(IOrderBusiness business, INubeFactBusiness nubeFactBusiness) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await business.GetAllAsync());

    [HttpGet("delivery")]
    public async Task<IActionResult> GetDeliveryOrders() => Ok(await business.GetDeliveryOrdersAsync());

    [HttpGet("rastreo/{id}")]
    public async Task<IActionResult> GetTracking(int id)
    {
        var result = await business.GetTrackingAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await business.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderRequest request)
    {
        try
        {
            var result = await business.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/invoice")]
    public async Task<IActionResult> GenerateInvoice(int id)
    {
        try
        {
            var result = await nubeFactBusiness.GenerateInvoiceAsync(id);
            return result.Success ? Ok(new { pdfUrl = result.PdfUrl }) : BadRequest(result.Error);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await business.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusUpdateRequest request)
    {
        var status = request.Status ?? request.Valor;
        if (string.IsNullOrEmpty(status)) return BadRequest("Status is required.");
        
        var success = await business.UpdateStatusAsync(id, status);
        return success ? Ok() : BadRequest("Invalid status or order not found.");
    }

    [HttpPatch("{id}/payment-status")]
    public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] StatusUpdateRequest request)
    {
        var status = request.Status ?? request.Valor;
        if (string.IsNullOrEmpty(status)) return BadRequest("Payment status is required.");

        var success = await business.UpdatePaymentStatusAsync(id, status);
        return success ? Ok() : BadRequest("Invalid payment status or order not found.");
    }

    [HttpPost("{id}/accept-delivery")]
    public async Task<IActionResult> AcceptDelivery(int id, [FromBody] AcceptDeliveryRequest request)
    {
        var success = await business.AcceptDeliveryOrderAsync(id, request.DeliveryUserId);
        return success ? Ok() : BadRequest("Order not found or could not be accepted.");
    }
}
