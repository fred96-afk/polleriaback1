using DbModel;
using DbModel.Tables;
using IRepository;

namespace Repository;

public class BannerRepository(PolleriaDbContext context) : BaseRepository<Banner>(context), IBannerRepository
{
}
