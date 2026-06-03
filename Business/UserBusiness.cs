using DbModel;
using AutoMapper;
using DbModel.Tables;
using IBusiness;
using IBusiness.Security;
using IRepository;
using Models.Users;
using IBusiness.Common;

namespace Business;

public class UserBusiness(
    IUserRepository repository, 
    IMapper mapper, 
    IPasswordHasher passwordHasher,
    IEmailService emailService) : IUserBusiness
{
    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return mapper.Map<IEnumerable<UserResponse>>(entities);
    }

    public async Task<UserResponse?> GetByIdAsync(int id)
    {
        var e = await repository.GetByIdAsync(id);
        return mapper.Map<UserResponse>(e);
    }

    public async Task<UserResponse> CreateAsync(UserRequest request)
    {
        var entity = mapper.Map<User>(request);
        
        // Hashear la contraseña con salt+pepper antes de guardar
        entity.PasswordHash = passwordHasher.HashPassword(request.Password);

        // Generar token de verificación
        entity.VerificationToken = Guid.NewGuid().ToString("N");
        entity.IsVerified = false;

        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();

        try
        {
            await emailService.SendVerificationEmailAsync(entity.Email, entity.Name, entity.VerificationToken);
        }
        catch (Exception ex)
        {
            // Log error but don't fail registration
            Console.WriteLine($"Error enviando correo de verificación: {ex.Message}");
        }
        
        return mapper.Map<UserResponse>(entity);
    }

    public async Task<bool> VerifyEmailAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;

        var users = await repository.FindAsync(u => 
            u.VerificationToken != null && 
            u.VerificationToken.Trim() == token.Trim());
            
        var user = users.FirstOrDefault();

        if (user == null) return false;

        user.IsVerified = true;
        user.VerificationToken = null; // Limpiar el token una vez usado

        repository.Update(user);
        return await repository.SaveChangesAsync() > 0;
    }

    public async Task RequestPasswordResetAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        var users = await repository.FindAsync(u => u.Email == email.Trim());
        var user = users.FirstOrDefault();
        if (user == null)
        {
            return;
        }

        user.PasswordResetToken = Guid.NewGuid().ToString("N");
        user.PasswordResetTokenExpiresAt = PeruTimeHelper.Now.AddHours(1);

        repository.Update(user);
        await repository.SaveChangesAsync();

        // Quitamos el try-catch para que el error sea visible en los logs o respuesta
        Console.WriteLine($"Solicitando envío de correo de recuperación para: {user.Email}");
        await emailService.SendPasswordResetEmailAsync(user.Email, user.Name, user.PasswordResetToken);
    }

    public async Task<PasswordResetResult> ResetPasswordAsync(string token, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
        {
            return PasswordResetResult.InvalidToken;
        }

        var users = await repository.FindAsync(u =>
            u.PasswordResetToken != null &&
            u.PasswordResetToken == token.Trim());

        var user = users.FirstOrDefault();
        if (user == null || user.PasswordResetTokenExpiresAt == null || user.PasswordResetTokenExpiresAt < PeruTimeHelper.Now)
        {
            return PasswordResetResult.InvalidToken;
        }

        // Verificar si la nueva contraseña es igual a la actual
        if (passwordHasher.VerifyPassword(newPassword, user.PasswordHash))
        {
            return PasswordResetResult.SamePassword;
        }

        user.PasswordHash = passwordHasher.HashPassword(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;

        repository.Update(user);
        return await repository.SaveChangesAsync() > 0 ? PasswordResetResult.Success : PasswordResetResult.Error;
    }

    public async Task<bool> UpdateAsync(int id, UserRequest request)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        mapper.Map(request, entity);

        if (!string.IsNullOrEmpty(request.Password))
        {
            entity.PasswordHash = passwordHasher.HashPassword(request.Password);
        }

        repository.Update(entity);
        return await repository.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        repository.Remove(entity);
        return await repository.SaveChangesAsync() > 0;
    }
}
