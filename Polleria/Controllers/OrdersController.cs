using IBusiness;
using Microsoft.AspNetCore.Mvc;
using Models.Orders;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderBusiness business, INubeFactBusiness nubeFactBusiness) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await business.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await business.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(OrderRequest request)
    {
        var result = await business.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id}/invoice")]
    public async Task<IActionResult> GenerateInvoice(int id)
    {
        var result = await nubeFactBusiness.GenerateInvoiceAsync(id);
        return result.Success ? Ok(new { pdfUrl = result.PdfUrl }) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await business.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
