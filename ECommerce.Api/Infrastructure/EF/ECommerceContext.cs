using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;
using RESTCountries.NET.Services;

namespace ECommerce.Api.Infrastructure.EF;

public class ECommerceContext : DbContext
{
    public DbSet<Admin> Admins { get; set; }

    public DbSet<Client> Clients { get; set; }
    public DbSet<ClientReview> ClientReviews { get; set; }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    public DbSet<Address> Addresses { get; set; }
    public DbSet<Country> Countries { get; set; }

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<ShopOrder> ShopOrders { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((context, _) =>
        {
            var client = context.Set<Client>()
                .FirstOrDefault(c => c.Email == "rp456@julio.com");
            if (client == null)
            {
                var newClient = new Client
                {
                    FirstName = "Ramón",
                    LastName = "Pruebas",
                    Email = "rp456@julio.com",
                    PhoneNumber = "849-666-7777",
                    PasswordHash = PasswordHasher.HashPassword("cliente123"),
                    BirthDate = new DateOnly(2005, 4, 13),
                    CreatedAt = DateTime.Parse("2026-01-26"),
                };
                context.Set<Client>().Add(newClient);
            }

            var admin = context.Set<Admin>()
                .FirstOrDefault(a => a.Email == "jp123@julio.com");
            if (admin != null)
            {
                var newAdmin = new Admin
                {
                    FirstName = "Julio",
                    LastName = "Pruebas",
                    Email = "jp123@julio.com",
                    PhoneNumber = "809-111-2222",
                    PasswordHash = PasswordHasher.HashPassword("admin123"),
                    BirthDate = new DateOnly(2001, 7, 22),
                    CreatedAt = DateTime.Parse("2026-01-26"),
                };
                context.Set<Admin>().Add(newAdmin);
            }

            var countries = context.Set<Country>();
            var countriesToAdd = RestCountriesService.GetAllCountriesNames()
                .Select(name => new Country { Name = name });

            if (!countries.Any())
                countries.AddRange(countriesToAdd);

            context.SaveChanges();
        });
    }
}