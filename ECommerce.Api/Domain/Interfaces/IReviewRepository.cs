using ECommerce.Api.Domain.Entities;

namespace ECommerce.Api.Domain.Interfaces;

public interface IReviewRepository : IRepository<ClientReview>
{
    Task<IEnumerable<ClientReview>> FindByClientId(int clientId);
    Task<IEnumerable<ClientReview>> FindByProductId(int productId);
}