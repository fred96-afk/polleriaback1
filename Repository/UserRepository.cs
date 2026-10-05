using DbModel;
using DbModel.Tables;
using IRepository;

namespace Repository;

public class UserRepository(PolleriaDbContext context) : BaseRepository<User>(context), IUserRepository { }
