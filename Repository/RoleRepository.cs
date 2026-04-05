using DbModel;
using DbModel.Tables;
using IRepository;

namespace Repository;

public class RoleRepository(PolleriaDbContext context) : BaseRepository<Role>(context), IRoleRepository { }
