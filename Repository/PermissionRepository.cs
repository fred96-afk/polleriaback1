using DbModel;
using DbModel.Tables;
using IRepository;

namespace Repository;

public class PermissionRepository(PolleriaDbContext context) : BaseRepository<Permission>(context), IPermissionRepository { }
