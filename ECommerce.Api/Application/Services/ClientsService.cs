using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Extensions;
using ECommerce.Api.Extensions.Mappings;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services;

public interface IClientsService
{
    Task<UserResponseDto?> GetByIdAsync(int clientId);
    Task<IEnumerable<UserResponseDto>> GetClientsAsync(string? search = null);
    Task<Result<UserResponseDto>> CreateAsync(CreateUserDto dto);
    Task<Result<UserResponseDto>> UpdateAsync(int clientId, UpdateUserDto dto);
    Task<UserResponseDto?> DeleteAsync(int clientId);
}

public class ClientsService(ECommerceContext db, IValidator<Client> validator) : IClientsService
{
    public async Task<UserResponseDto?> GetByIdAsync(int clientId)
    {
        var client = await db.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        return client?.GetDto();
    }

    public async Task<IEnumerable<UserResponseDto>> GetClientsAsync(string? search = null)
    {
        var clients = await db.Clients.ToListAsync();
        return clients
            .Where(c => ClientMatches(c, search))
            .Select(c => c.GetDto());
    }

    private bool ClientMatches(Client client, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return true;

        var fullName = client.FirstName + " " + client.LastName;
        return fullName.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    public async Task<Result<UserResponseDto>> CreateAsync(CreateUserDto dto)
    {
        var client = dto.GetEntity();

        var validation = await validator.ValidateAsync(client);

        if (!validation.IsValid)
            return Errors.ValidationError(validation.ToDictionary());
        
        await db.Clients.AddAsync(client);
        await db.SaveChangesAsync();
        
        return client.GetDto();
    }

    public async Task<Result<UserResponseDto>> UpdateAsync(int clientId, UpdateUserDto dto)
    {
        var client = await db.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (client == null)
            return Errors.NotFound();

        var updated = client.GetUpdated(dto);
        
        var validation = await validator.ValidateAsync(updated);
        if (!validation.IsValid)
            return Errors.ValidationError(validation.ToDictionary());
        
        ApplyClientUpdate(client, updated);

        await db.SaveChangesAsync();
        
        return client.GetDto();
    }

    private void ApplyClientUpdate(Client client, Client updated)
    {
        client.Id = updated.Id;
        client.FirstName = updated.FirstName;
        client.LastName = updated.LastName;
        client.Email = updated.Email;
        client.PhoneNumber = updated.PhoneNumber;
        client.PasswordHash = updated.PasswordHash;
        client.BirthDate = updated.BirthDate;
        client.CreatedAt = updated.CreatedAt;
    }

    public async Task<UserResponseDto?> DeleteAsync(int clientId)
    {
        var client = await db.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (client == null)
            return null;

        db.Clients.Remove(client);
        await db.SaveChangesAsync();

        return client.GetDto();
    }
}