using ECommerce.Api.Domain.Entities;

namespace ECommerce.Api.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> FindByName(string name);
    Task<IEnumerable<Product>> FindByCategoryName(string categoryName);
    Task<IEnumerable<Product>> FindByCategoryId(int categoryId);
}