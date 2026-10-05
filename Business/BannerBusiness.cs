using AutoMapper;
using DbModel.Tables;
using IBusiness;
using IRepository;
using Models.Banners;
using Models.Common;

namespace Business;

public class BannerBusiness(
    IBannerRepository repository,
    ICloudinaryService cloudinaryService,
    IMapper mapper) : IBannerBusiness
{
    public async Task<IEnumerable<BannerResponse>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return mapper.Map<IEnumerable<BannerResponse>>(entities);
    }

    public async Task<PagedResponse<BannerResponse>> GetPagedAsync(PaginationParams pagination)
    {
        var (items, totalCount) = await repository.GetPagedAsync(pagination.PageNumber, pagination.PageSize);
        var dtos = mapper.Map<IEnumerable<BannerResponse>>(items);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

        return new PagedResponse<BannerResponse>(dtos, totalCount, totalPages, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<BannerResponse?> GetByIdAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        return entity == null ? null : mapper.Map<BannerResponse>(entity);
    }

    public async Task<BannerResponse> CreateAsync(BannerRequest request)
    {
        string imageUrl = string.Empty;
        if (request.Image != null)
        {
            imageUrl = await cloudinaryService.UploadImageAsync(request.Image, "banners");
        }

        var entity = mapper.Map<Banner>(request);
        entity.ImageUrl = imageUrl;

        await repository.AddAsync(entity);
        await repository.SaveChangesAsync();

        return mapper.Map<BannerResponse>(entity);
    }

    public async Task<bool> UpdateAsync(int id, BannerRequest request)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        if (request.Image != null)
        {
            entity.ImageUrl = await cloudinaryService.UploadImageAsync(request.Image, "banners");
        }

        entity.Title = request.Title;
        entity.Subtitle = request.Subtitle;
        entity.LinkUrl = request.LinkUrl;
        entity.Order = request.Order;
        entity.IsActive = request.IsActive;

        repository.Update(entity);
        return await repository.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null) return false;

        repository.Remove(entity);
        return await repository.SaveChangesAsync() > 0;
    }
}
