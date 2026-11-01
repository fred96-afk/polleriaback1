using DbModel.Tables;

namespace IRepository;

public interface ITableRepository : IBaseRepository<RestaurantTable>
{
    Task<RestaurantTable?> GetByNumberAsync(string number);
}
