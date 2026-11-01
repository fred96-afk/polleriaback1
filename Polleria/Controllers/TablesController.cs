using IBusiness;
using Microsoft.AspNetCore.Mvc;
using Models.Tables;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController(ITableBusiness business) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? mapId = null) 
        => Ok(await business.GetAllAsync(mapId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await business.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("number/{number}")]
    public async Task<IActionResult> GetByNumber(string number)
    {
        var result = await business.GetByNumberAsync(number);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TableRequest request)
    {
        var result = await business.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TableRequest request)
    {
        var success = await business.UpdateAsync(id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpPatch("{id}/position")]
    public async Task<IActionResult> UpdatePosition(int id, [FromBody] TablePositionRequest request)
    {
        var success = await business.UpdatePositionAsync(id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await business.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Emite una notificación en tiempo real indicando que la mesa está llamando al mozo.
    /// </summary>
    [HttpPost("call-waiter")]
    public async Task<IActionResult> CallWaiter([FromBody] TableNotificationRequest request)
    {
        var success = await business.NotifyCallWaiterAsync(request.TableNumber, request.Message);
        return success
            ? Ok(new { success = true, message = "Notificación de llamada de mozo enviada." })
            : NotFound(new { message = $"No se encontró la mesa '{request.TableNumber}'." });
    }

    /// <summary>
    /// Emite una notificación en tiempo real indicando que la mesa solicitó la cuenta.
    /// </summary>
    [HttpPost("request-bill")]
    public async Task<IActionResult> RequestBill([FromBody] TableNotificationRequest request)
    {
        var success = await business.NotifyRequestBillAsync(request.TableNumber, request.Message);
        return success
            ? Ok(new { success = true, message = "Notificación de solicitud de cuenta enviada." })
            : NotFound(new { message = $"No se encontró la mesa '{request.TableNumber}'." });
    }

    /// <summary>
    /// Emite una alerta genérica en tiempo real asociada a una mesa.
    /// </summary>
    [HttpPost("alert")]
    public async Task<IActionResult> Alert([FromBody] TableAlertRequest request)
    {
        var success = await business.NotifyTableAlertAsync(request.TableNumber, request.Message, request.Type);
        return success
            ? Ok(new { success = true, message = "Alerta de mesa enviada." })
            : NotFound(new { message = $"No se encontró la mesa '{request.TableNumber}'." });
    }
}
