using DbModel;
using DbModel.Tables;
using IRepository;

namespace Repository;

public class SideRepository(PolleriaDbContext context) : BaseRepository<Side>(context), ISideRepository { }
