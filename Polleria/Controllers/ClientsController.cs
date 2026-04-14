using IBusiness;
using Microsoft.AspNetCore.Mvc;
using Models.Clients;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IClientBusiness business) : ControllerBase
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
    public async Task<IActionResult> Create(ClientRequest request)
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ClientRequest request)
    {
        try
        {
            var success = await business.UpdateAsync(id, request);
            return success ? NoContent() : NotFound();
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
}
