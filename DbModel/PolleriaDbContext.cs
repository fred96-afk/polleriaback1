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

    // --- English Entities (New Model) ---
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermission { get; set; }
    public DbSet<Side> Sides { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Banner> Banners { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);

        // Seeding
        PermissionSeed.Seed(modelBuilder);
        RoleSeed.Seed(modelBuilder);
        RolePermissionSeed.Seed(modelBuilder);
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
