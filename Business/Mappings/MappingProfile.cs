using AutoMapper;
using DbModel.Tables;
using Models.Clients;
using Models.Products;
using Models.Sides;
using Models.Users;
using Models.Roles;
using Models.Orders;

namespace Business.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Client, ClientResponse>();
        CreateMap<ClientRequest, Client>();

        CreateMap<Product, ProductResponse>();
        CreateMap<ProductRequest, Product>();

        CreateMap<Side, SideResponse>();
        CreateMap<SideRequest, Side>();

        CreateMap<User, UserResponse>();
        CreateMap<UserRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        CreateMap<Role, RoleResponse>();

        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.Details, opt => opt.Ignore()); // Se mapea manualmente o con sub-mapeo
        
        CreateMap<OrderDetail, OrderDetailResponse>();
    }
}
