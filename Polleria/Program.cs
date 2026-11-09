using System.Text;
using System.Threading.RateLimiting;
using Business;
using Business.Mappings;
using Mapster;
using MapsterMapper;
using Business.Security;
using DbModel;
using IBusiness;
using IBusiness.Security;
using IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Models;
using Models.RabbitMQ;
using Business.RabbitMQ;
using Business.Services;
using Business.Workers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Soporte para Secret Files en Render (/etc/secrets/appsettings.json)
builder.Configuration.AddJsonFile("/etc/secrets/appsettings.json", optional: true, reloadOnChange: true);

// --- 1. Database Connection ---
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<PolleriaDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("connectionstringsomee"),
        b =>
        {
            b.MigrationsAssembly("Polleria");
            b.CommandTimeout(120);
        });
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// --- 1.1 CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://polleriafront1.vercel.app", "https://polleriafront-one.vercel.app")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

// --- 2. Mapster ---
var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
typeAdapterConfig.Scan(typeof(MappingProfile).Assembly);
builder.Services.AddSingleton(typeAdapterConfig);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// --- 3. Repositories ---
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<ISideRepository, SideRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IBannerRepository, BannerRepository>();
builder.Services.AddScoped<ITableRepository, TableRepository>();

// --- 4. Business Services ---
builder.Services.AddScoped<IClientBusiness, ClientBusiness>();
builder.Services.AddScoped<IOrderBusiness, OrderBusiness>();
builder.Services.AddScoped<IProductBusiness, ProductBusiness>();
builder.Services.AddScoped<ICategoryBusiness, CategoryBusiness>();
builder.Services.AddScoped<IRoleBusiness, RoleBusiness>();
builder.Services.AddScoped<ISideBusiness, SideBusiness>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IBannerBusiness, BannerBusiness>();
builder.Services.AddScoped<ICatalogoBusiness, CatalogoBusiness>();
builder.Services.AddScoped<IMercadoPagoBusiness, MercadoPagoBusiness>();
builder.Services.AddScoped<INubeFactBusiness, NubeFactBusiness>();
builder.Services.AddHttpClient<IChatBusiness, GeminiChatBusiness>(client =>
{
    client.Timeout = Timeout.InfiniteTimeSpan;
});
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddHttpClient<IEmailService, EmailService>();
builder.Services.AddSingleton<IPusherService, PusherService>();
builder.Services.Configure<PusherSettings>(builder.Configuration.GetSection("Pusher"));
builder.Services.AddScoped<IReportBusiness, ReportBusiness>();
builder.Services.AddScoped<IDashboardBusiness, DashboardBusiness>();
builder.Services.AddScoped<ITableBusiness, TableBusiness>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// --- 4.1 RabbitMQ & Report Workers ---
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IRabbitMQConnectionManager, RabbitMQConnectionManager>();
builder.Services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();
builder.Services.AddSingleton<IReportStorageService, ReportStorageService>();

// Workers en segundo plano para generación de reportes
builder.Services.AddHostedService<ExcelReportWorker>();
builder.Services.AddHostedService<PdfReportWorker>();

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
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (string.IsNullOrEmpty(context.Token))
            {
                if (context.Request.Cookies.TryGetValue("auth_token", out var token) && !string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                else if (context.Request.Cookies.TryGetValue("token", out var legacyToken) && !string.IsNullOrEmpty(legacyToken))
                {
                    context.Token = legacyToken;
                }
            }
            return Task.CompletedTask;
        }
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1),
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("EmailTemplateTests", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 1,
                Window = TimeSpan.FromMinutes(15),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Pollería API");
        options.WithTheme(ScalarTheme.Moon);
    });
    app.MapGet("/swagger", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowAngularApp");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
