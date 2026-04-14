using IBusiness;
using Microsoft.AspNetCore.Mvc;
using Models.Users;
using DbModel.Tables;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserBusiness userBusiness) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        var users = await userBusiness.GetAllAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Create([FromBody] UserRequest request)
    {
        var user = await userBusiness.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = user.Id }, user);
    }
}
