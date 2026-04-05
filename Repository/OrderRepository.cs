using DbModel;
using DbModel.Tables;
using IRepository;

namespace Repository;

public class OrderRepository(PolleriaDbContext context) : BaseRepository<Order>(context), IOrderRepository { }
