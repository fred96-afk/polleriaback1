using DbModel;
using DbModel.Tables;
using IRepository;

namespace Repository;

public class ProductRepository(PolleriaDbContext context) : BaseRepository<Product>(context), IProductRepository { }
