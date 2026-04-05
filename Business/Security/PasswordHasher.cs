using IBusiness.Security;
using Microsoft.Extensions.Configuration;

namespace Business.Security;

public class PasswordHasher : IPasswordHasher
{
    private readonly string _pepper;

    public PasswordHasher(IConfiguration configuration)
    {
        _pepper = configuration["PasswordSettings:Pepper"] 
                  ?? throw new InvalidOperationException("Password Pepper not found in configuration.");
    }

    public string HashPassword(string password)
    {
        // Combinamos la contraseña con el Pepper antes de hashear
        // BCrypt generará internamente su propio Salt único además de esto.
        string saltedPassword = password + _pepper;
        return BCrypt.Net.BCrypt.HashPassword(saltedPassword);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        string saltedPassword = password + _pepper;
        return BCrypt.Net.BCrypt.Verify(saltedPassword, hashedPassword);
    }
}
