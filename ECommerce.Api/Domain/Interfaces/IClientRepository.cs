using ECommerce.Api.Domain.Entities;

namespace ECommerce.Api.Domain.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    Task<Client?> GetByEmail(string email);
    Task<Client?> GetByPhoneNumber(string phoneNumber);
    Task<IEnumerable<Client>> FindByName(string name);
}