using DbModel;
using DbModel.Tables;
using IRepository;

namespace Repository;

public class ClientRepository(PolleriaDbContext context) : BaseRepository<Client>(context), IClientRepository { }
