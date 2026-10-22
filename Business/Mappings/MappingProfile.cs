using Mapster;
using DbModel.Tables;
using Models.Clients;
using Models.Products;
using Models.Sides;
using Models.Users;
using Models.Roles;
using Models.Orders;
using Models.Categories;
using Models.Banners;

namespace Business.Mappings;

public class MappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Client, ClientResponse>();
        config.NewConfig<ClientRequest, Client>();

        config.NewConfig<Product, ProductResponse>();
        config.NewConfig<ProductRequest, Product>();

        config.NewConfig<Category, CategoryResponse>();
        config.NewConfig<CategoryRequest, Category>();

        config.NewConfig<Side, SideResponse>();
        config.NewConfig<SideRequest, Side>();

        config.NewConfig<User, UserResponse>();
        config.NewConfig<UserRequest, User>()
            .Ignore(dest => dest.PasswordHash);

        config.NewConfig<Role, RoleResponse>();

        config.NewConfig<Order, OrderResponse>()
            .Ignore(dest => dest.Details); // Se mapea manualmente o con sub-mapeo
        
        config.NewConfig<OrderDetail, OrderDetailResponse>();

        config.NewConfig<Banner, BannerResponse>();
        config.NewConfig<BannerRequest, Banner>()
            .Ignore(dest => dest.ImageUrl);
    }
}
