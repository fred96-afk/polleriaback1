using System.Text;
using Business;
using Business.Mappings;
using Business.Security;
using DbModel;
using IBusiness;
using IBusiness.Security;
using IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Database Connection ---
builder.Services.AddDbContext<PolleriaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("connectionstringsomee"),
        b => b.MigrationsAssembly("Polleria")));

// --- 1.1 CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://polleriafront.vercel.app")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// --- 2. AutoMapper ---
builder.Services.AddAutoMapper(typeof(MappingProfile));

// --- 3. Repositories ---
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ISideRepository, SideRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

// --- 4. Business Services ---
builder.Services.AddScoped<IClientBusiness, ClientBusiness>();
builder.Services.AddScoped<IOrderBusiness, OrderBusiness>();
builder.Services.AddScoped<IProductBusiness, ProductBusiness>();
builder.Services.AddScoped<ICategoryBusiness, CategoryBusiness>();
builder.Services.AddScoped<IRoleBusiness, RoleBusiness>();
builder.Services.AddScoped<ISideBusiness, SideBusiness>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IMercadoPagoBusiness, MercadoPagoBusiness>();
builder.Services.AddScoped<INubeFactBusiness, NubeFactBusiness>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddHttpClient();

// --- 5. Security Services ---
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();

// --- 6. JWT Authentication ---
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
