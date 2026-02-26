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
    Task<bool> ClientExistsAsync(int clientId);
    Task<UserResponseDto?> GetByIdAsync(int clientId);
    Task<IEnumerable<UserResponseDto>> GetClientsAsync(string? search = null);
    Task<Result<UserResponseDto>> CreateAsync(CreateUserDto dto);
    Task<Result<UserResponseDto>> UpdateAsync(int clientId, UpdateUserDto dto);
    Task<UserResponseDto?> DeleteAsync(int clientId);
}

public class ClientsService(ECommerceContext context, IValidator<Client> validator) : IClientsService
{
    public async Task<bool> ClientExistsAsync(int clientId)
        => await context.Clients.AnyAsync(c => c.Id == clientId);

    public async Task<UserResponseDto?> GetByIdAsync(int clientId)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        return client?.GetDto();
    }

    public async Task<IEnumerable<UserResponseDto>> GetClientsAsync(string? search = null)
    {
        var clients = await context.Clients.ToListAsync();
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

        var validationResult = await Validate(client);
        if (!validationResult.IsSuccess)
            return Errors.ValidationError(validationResult.Error!.Details);

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        return client.GetDto();
    }

    public async Task<Result<UserResponseDto>> UpdateAsync(int clientId, UpdateUserDto dto)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (client == null)
            return Errors.NotFound();

        var updated = client.GetUpdated(dto);

        var validationResult = await Validate(updated);
        if (!validationResult.IsSuccess)
            return Errors.ValidationError(validationResult.Error!.Details);

        PropertyCopier.Mirror(updated, client);

        await context.SaveChangesAsync();

        return client.GetDto();
    }

    public async Task<UserResponseDto?> DeleteAsync(int clientId)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (client == null)
            return null;

        context.Clients.Remove(client);
        await context.SaveChangesAsync();

        return client.GetDto();
    }

    private async Task<Result> Validate(Client client)
    {
        var validation = await validator.ValidateAsync(client);
        if (!validation.IsValid)
        {
            return Errors.ValidationError(validation.ToDictionary());
        }

        var emailIsDuplicate = await EmailIsDuplicate(client);
        var phoneNumberIsDuplicate = await PhoneNumberIsDuplicate(client);

        if (!emailIsDuplicate && !phoneNumberIsDuplicate) 
            return Result.Success();
        
        var errorDetails = new Dictionary<string, string[]>();
        
        if (emailIsDuplicate)
            errorDetails.Add(nameof(client.Email), ["Email address already in use"]);
        if (phoneNumberIsDuplicate)
            errorDetails.Add(nameof(client.PhoneNumber), ["Phone number already in use"]);

        return Errors.ValidationError(errorDetails);
    }

    private async Task<bool> EmailIsDuplicate(Client client)
        => await context.Clients.AnyAsync(c => c.Email == client.Email && c.Id != client.Id);

    private async Task<bool> PhoneNumberIsDuplicate(Client client)
        => await context.Clients.AnyAsync(c => c.PhoneNumber == client.PhoneNumber && c.Id != client.Id);
}