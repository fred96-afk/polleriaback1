using DbModel.Tables;

namespace IRepository;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<IEnumerable<Role>> GetAllWithPermissionsAsync();
    Task<Role?> GetByIdWithPermissionsAsync(int id);
}
