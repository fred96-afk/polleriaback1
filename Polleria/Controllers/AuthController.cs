using IBusiness;
using IRepository;
using IBusiness.Security;
using Microsoft.AspNetCore.Mvc;
using Models.Users;

namespace Polleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IUserBusiness userBusiness,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher,
    IJwtService jwtService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var users = await userRepository.FindAsync(u => u.Email == request.Email);
        var user = users.FirstOrDefault();

        if (user == null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized("Email o contraseña incorrectos.");
        }

        var role = await roleRepository.GetByIdAsync(user.RoleId);
        var roleName = role?.Name ?? "User";

        var token = jwtService.GenerateToken(user, roleName);
        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRequest request)
    {
        var users = await userRepository.FindAsync(u => u.Email == request.Email);
        if (users.Any())
        {
            return BadRequest("El email ya está registrado.");
        }

        var user = await userBusiness.CreateAsync(request);
        return Ok(user);
    }
}

public record LoginRequest(string Email, string Password);
