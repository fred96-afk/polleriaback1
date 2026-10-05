using IBusiness;
using Microsoft.AspNetCore.Mvc;
using Models.Banners;
using Models.Common;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BannersController(IBannerBusiness business) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await business.GetAllAsync());

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PaginationParams pagination) 
        => Ok(await business.GetPagedAsync(pagination));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await business.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] BannerRequest request)
    {
        var result = await business.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] BannerRequest request)
    {
        var success = await business.UpdateAsync(id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await business.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
