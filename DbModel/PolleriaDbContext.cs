using Microsoft.EntityFrameworkCore;
using DbModel.Tables;
using DbModel.Seeds;

namespace DbModel;

public class PolleriaDbContext : DbContext
{
    public PolleriaDbContext(DbContextOptions<PolleriaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Side> Sides { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Banner> Banners { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Invoice> Invoices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Relationships (Fluent API) ---

        // Payment - Order
        modelBuilder.Entity<Payment>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Invoice - Order
        modelBuilder.Entity<Invoice>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // OrderDetail - Side
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Side)
            .WithMany()
            .HasForeignKey(od => od.SideId)
            .OnDelete(DeleteBehavior.SetNull);

        // OrderDetail - Product
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany()
            .HasForeignKey(od => od.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Product - Category
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seeding
        RoleSeed.Seed(modelBuilder);
        UserSeed.Seed(modelBuilder);
        ClientSeed.Seed(modelBuilder);
        SideSeed.Seed(modelBuilder);
        ProductSeed.Seed(modelBuilder);
        BannerSeed.Seed(modelBuilder);
        OrderSeed.Seed(modelBuilder);
        OrderDetailSeed.Seed(modelBuilder);
        PaymentSeed.Seed(modelBuilder);
        InvoiceSeed.Seed(modelBuilder);
    }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }
        }