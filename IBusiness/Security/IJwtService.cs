using DbModel.Tables;

namespace IBusiness.Security;

public interface IJwtService
{
    string GenerateToken(User user, string roleName, IEnumerable<string> permissions);
}
