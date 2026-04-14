using IBusiness;
using IRepository;
using IBusiness.Security;
using Microsoft.AspNetCore.Mvc;
using Models.Users;
using DbModel.Tables; // Added for Role access

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

        if (!user.IsVerified)
        {
            return BadRequest("Debes verificar tu correo electrónico antes de iniciar sesión.");
        }

        var role = await roleRepository.GetByIdAsync(user.RoleId);
        var roleName = role?.Name ?? "User"; // Default to "User" if role not found

        // For client login, we can optionally check if the role is not 'Admin' or 'Waiter'/'Staff'
        if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
            roleName.Equals("Waiter", StringComparison.OrdinalIgnoreCase) || 
            roleName.Equals("Mozo", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Please use the /adminlogin endpoint for administrative access.");
        }

        var token = jwtService.GenerateToken(user, roleName);
        return Ok(new { Token = token, Role = roleName });
    }

    [HttpPost("adminlogin")]
    public async Task<IActionResult> AdminLogin([FromBody] LoginRequest request)
    {
        var users = await userRepository.FindAsync(u => u.Email == request.Email);
        var user = users.FirstOrDefault();

        if (user == null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized("Email o contraseña incorrectos.");
        }

        if (!user.IsVerified)
        {
            return BadRequest("Debes verificar tu correo electrónico antes de iniciar sesión.");
        }

        var role = await roleRepository.GetByIdAsync(user.RoleId);
        var roleName = role?.Name ?? "User";

        // Only deny login for 'Client' roles. All other roles are allowed.
        if (roleName.Equals("Client", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized("Acceso no autorizado: los clientes deben usar el endpoint de login normal.");
        }

        var token = jwtService.GenerateToken(user, roleName);
        return Ok(new { Token = token, Role = roleName });
    }


    [HttpPost("registerclient")]
    public async Task<IActionResult> RegisterClient([FromBody] UserRequest request)
    {
        var users = await userRepository.FindAsync(u => u.Email == request.Email);
        var existingUser = users.FirstOrDefault();

        if (existingUser != null)
        {
            if (existingUser.IsVerified)
            {
                return BadRequest("El email ya está registrado y verificado.");
            }
            
            // Si el usuario existe pero no está verificado, lo eliminamos para permitir un nuevo registro limpio
            // (esto renovará el token de verificación y permitirá corregir datos si se equivocó)
            userRepository.Remove(existingUser);
            await userRepository.SaveChangesAsync();
        }

        // Assign a default client role to the new user
        var clientRole = (await roleRepository.FindAsync(r => r.Name == "Client")).FirstOrDefault();
        if (clientRole == null)
        {
            // If "Client" role doesn't exist, we might need to create it or handle this error.
            // For now, let's assume it exists or throw an error.
            return StatusCode(500, "Client role not found. Please configure roles.");
        }

        // Create a new UserRequest with the Client RoleId
        var clientRequest = request with { RoleId = clientRole.Id };
        var user = await userBusiness.CreateAsync(clientRequest);
        return Ok(user);
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        var success = await userBusiness.VerifyEmailAsync(token);
        if (!success)
        {
            return BadRequest("Token de verificación inválido o expirado.");
        }

        return Ok("Correo electrónico verificado con éxito. Ya puedes iniciar sesión.");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await userBusiness.RequestPasswordResetAsync(request.Email);
        return Ok(new { message = "Si el correo existe, se envio un enlace para restablecer la contraseña." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
        {
            return BadRequest(new { message = "La nueva contraseña debe tener al menos 6 caracteres." });
        }

        var success = await userBusiness.ResetPasswordAsync(request.Token, request.NewPassword);
        if (!success)
        {
            return BadRequest(new { message = "El token de recuperación es inválido o expiró." });
        }

        return Ok(new { message = "La contraseña fue actualizada correctamente." });
    }
}

public record LoginRequest(string Email, string Password);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NewPassword);

