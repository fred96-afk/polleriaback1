using Models.Users;

namespace IBusiness;

public interface IUserBusiness
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse?> GetByIdAsync(int id);
    Task<UserResponse> CreateAsync(UserRequest request);
    Task<bool> UpdateAsync(int id, UserRequest request);
    Task<bool> DeleteAsync(int id);
    Task<bool> VerifyEmailAsync(string token);
    Task RequestPasswordResetAsync(string email);
    Task<bool> ResetPasswordAsync(string token, string newPassword);
}
