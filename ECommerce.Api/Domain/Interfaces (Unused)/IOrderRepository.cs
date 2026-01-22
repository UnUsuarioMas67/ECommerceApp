using ECommerce.Api.Domain.Entities;

namespace ECommerce.Api.Domain.Interfaces;

public interface IOrderRepository : IRepository<ShopOrder>
{
    Task<IEnumerable<ShopOrder>> FindByClientId(int clientId);
    Task<IEnumerable<ShopOrder>> FindByProductId(int productId);
}