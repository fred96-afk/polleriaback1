using Models.Banners;
using Models.Common;

namespace IBusiness;

public interface IBannerBusiness
{
    Task<IEnumerable<BannerResponse>> GetAllAsync();
    Task<PagedResponse<BannerResponse>> GetPagedAsync(PaginationParams pagination);
    Task<BannerResponse?> GetByIdAsync(int id);
    Task<BannerResponse> CreateAsync(BannerRequest request);
    Task<bool> UpdateAsync(int id, BannerRequest request);
    Task<bool> DeleteAsync(int id);
}
