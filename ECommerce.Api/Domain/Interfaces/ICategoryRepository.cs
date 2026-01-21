using ECommerce.Api.Domain.Entities;

namespace ECommerce.Api.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{ 
    Task<IEnumerable<Category>> FindByName(string categoryName);
}