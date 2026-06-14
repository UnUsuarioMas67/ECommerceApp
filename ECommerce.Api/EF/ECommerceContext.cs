using ECommerce.Api.EF.Seeding;
using ECommerce.Api.Entities;
using Microsoft.EntityFrameworkCore;
using RESTCountries.NET.Services;

namespace ECommerce.Api.EF;

public class ECommerceContext : DbContext
{
    public DbSet<Admin> Admins { get; set; }

    public DbSet<Client> Clients { get; set; }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    public DbSet<Address> Addresses { get; set; }
    public DbSet<Country> Countries { get; set; }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<ShopOrder> ShopOrders { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }

    public DbSet<Payment> Payments { get; set; }

    public DbSet<ClientRefreshToken> ClientRefreshTokens { get; set; }
    public DbSet<AdminRefreshToken> AdminRefreshTokens { get; set; }

    public ECommerceContext()
    {
    }

    public ECommerceContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ECommerceContext).Assembly);
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.AddSeedingToDbContext();
    // }
}