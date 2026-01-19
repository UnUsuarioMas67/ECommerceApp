using ECommerce.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Infrastructure.EF;

public class ECommerceContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserReview> UserReviews { get; set; }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Country> Countries { get; set; }
    
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    
    public DbSet<ShopOrder> ShopOrders { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    
    public ECommerceContext() { }
    
    public ECommerceContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ECommerceContext).Assembly);
    }
}