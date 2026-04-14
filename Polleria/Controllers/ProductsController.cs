using IBusiness;
using Microsoft.AspNetCore.Mvc;
using Models.Products;
using Models.Common;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductBusiness business) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await business.GetAllAsync());

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] PaginationParams pagination, [FromQuery] string? term) 
        => Ok(await business.GetPagedAsync(pagination, term));

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term) => Ok(await business.SearchAsync(term));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await business.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ProductRequest request)
    {
        var result = await business.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] ProductRequest request)
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