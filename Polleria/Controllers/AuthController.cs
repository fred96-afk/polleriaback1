using IBusiness;
using IRepository;
using IBusiness.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Models.Users;
using DbModel.Tables; // Added for Role access
using IBusiness.Common;

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
    private void SetAuthCookie(string token)
    {
        var isHttps = Request.IsHttps;
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(90), // 1 hora y media (90 minutos)
            IsEssential = true,
            Path = "/"
        };

        Response.Cookies.Append("auth_token", token, cookieOptions);
    }

    private void RemoveAuthCookie()
    {
        var isHttps = Request.IsHttps;
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Path = "/"
        };

        Response.Cookies.Delete("auth_token", cookieOptions);
    }

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
        var roleName = role?.Name ?? "User"; // Default to "User" if role not found

        // For client login, we can optionally check if the role is not 'Admin' or 'Waiter'/'Staff'
        if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) || 
            roleName.Equals("Waiter", StringComparison.OrdinalIgnoreCase) || 
            roleName.Equals("Mozo", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Please use the /adminlogin endpoint for administrative access.");
        }

        var roleWithPermissions = await roleRepository.GetByIdWithPermissionsAsync(user.RoleId);
        var permissions = roleWithPermissions?.RolePermissions.Select(rp => rp.Permission.Code) ?? Enumerable.Empty<string>();

        var token = jwtService.GenerateToken(user, roleName, permissions);
        SetAuthCookie(token);

        var userDto = new
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = roleName,
            RoleId = user.RoleId,
            Permissions = permissions
        };

        return Ok(new
        {
            Token = token,
            User = userDto,
            Role = roleName,
            Permissions = permissions
        });
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

        var role = await roleRepository.GetByIdAsync(user.RoleId);
        var roleName = role?.Name ?? "User";

        // Only deny login for 'Client' roles. All other roles are allowed.
        if (roleName.Equals("Client", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized("Acceso no autorizado: los clientes deben usar el endpoint de login normal.");
        }

        var roleWithPermissions = await roleRepository.GetByIdWithPermissionsAsync(user.RoleId);
        var permissions = roleWithPermissions?.RolePermissions.Select(rp => rp.Permission.Code) ?? Enumerable.Empty<string>();

        var token = jwtService.GenerateToken(user, roleName, permissions);
        SetAuthCookie(token);

        var userDto = new
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = roleName,
            RoleId = user.RoleId,
            Permissions = permissions
        };

        return Ok(new
        {
            Token = token,
            User = userDto,
            Role = roleName,
            Permissions = permissions
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        RemoveAuthCookie();
        return Ok(new { message = "Sesión cerrada correctamente." });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Usuario no identificado.");
        }

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized("Usuario no encontrado.");
        }

        var role = await roleRepository.GetByIdAsync(user.RoleId);
        var roleName = role?.Name ?? "User";

        var roleWithPermissions = await roleRepository.GetByIdWithPermissionsAsync(user.RoleId);
        var permissions = roleWithPermissions?.RolePermissions.Select(rp => rp.Permission.Code) ?? Enumerable.Empty<string>();

        return Ok(new
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = roleName,
            RoleId = user.RoleId,
            Permissions = permissions
        });
    }


    [HttpPost("registerclient")]
    public async Task<IActionResult> RegisterClient([FromBody] UserRequest request)
    {
        var users = await userRepository.FindAsync(u => u.Email == request.Email);
        var existingUser = users.FirstOrDefault();

        if (existingUser != null)
        {
            return BadRequest("El email ya está registrado.");
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

        var result = await userBusiness.ResetPasswordAsync(request.Token, request.NewPassword);
        
        return result switch
        {
            PasswordResetResult.Success => Ok(new { message = "La contraseña fue actualizada correctamente." }),
            PasswordResetResult.SamePassword => BadRequest(new { message = "No puedes repetir la misma contraseña." }),
            PasswordResetResult.InvalidToken => BadRequest(new { message = "El token de recuperación es inválido o expiró." }),
            _ => BadRequest(new { message = "Ocurrió un error al restablecer la contraseña." })
        };
    }

    private static bool IsInternalRole(string roleName)
    {
        return roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            || roleName.Equals("Waiter", StringComparison.OrdinalIgnoreCase)
            || roleName.Equals("Mozo", StringComparison.OrdinalIgnoreCase)
            || roleName.Equals("Delivery", StringComparison.OrdinalIgnoreCase);
    }
}

public record LoginRequest(string Email, string Password);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NewPassword);

