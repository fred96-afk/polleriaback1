using DbModel;
using DbModel.Tables;
using IRepository;

using Microsoft.EntityFrameworkCore;

namespace Repository;

public class RoleRepository(PolleriaDbContext context) : BaseRepository<Role>(context), IRoleRepository
{
    public async Task<IEnumerable<Role>> GetAllWithPermissionsAsync()
    {
        return await _dbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Role?> GetByIdWithPermissionsAsync(int id)
    {
        return await _dbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}
