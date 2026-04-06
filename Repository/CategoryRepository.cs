using DbModel;
using DbModel.Tables;
using IRepository;

namespace Repository;

public class CategoryRepository(PolleriaDbContext context) : BaseRepository<Category>(context), ICategoryRepository { }