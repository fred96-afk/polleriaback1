using IBusiness;
using Microsoft.AspNetCore.Mvc;
using Models.Users;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserBusiness userBusiness) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var users = await userBusiness.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await userBusiness.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] UserRequest request)
    {
        var user = await userBusiness.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserRequest request)
    {
        var success = await userBusiness.UpdateAsync(id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await userBusiness.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }
}
