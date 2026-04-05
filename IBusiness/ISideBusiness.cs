using Models.Sides;

namespace IBusiness;

public interface ISideBusiness
{
    Task<IEnumerable<SideResponse>> GetAllAsync();
    Task<SideResponse?> GetByIdAsync(int id);
    Task<SideResponse> CreateAsync(SideRequest request);
    Task<bool> UpdateAsync(int id, SideRequest request);
    Task<bool> DeleteAsync(int id);
}
