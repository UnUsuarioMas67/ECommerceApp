using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using ECommerce.Api.Shared.Errors;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.DataAccess;

public interface IClientsService
{
    Task<bool> EntryExistsAsync(int clientId);
    Task<UserResponseDto?> GetByIdAsync(int clientId);
    Task<IEnumerable<UserResponseDto>> GetManyAsync(PaginationQuery pagination, string? search = null);
    Task<Result<UserResponseDto>> CreateAsync(UserCreateDto dto);
    Task<Result<UserResponseDto>> UpdateAsync(int clientId, UserUpdateDto dto);
    Task<UserResponseDto?> DeleteAsync(int clientId);
}

public class ClientsService(ECommerceContext context, IValidator<Client> validator, ClientMapper mapper)
    : IClientsService
{
    public async Task<bool> EntryExistsAsync(int clientId)
        => await context.Clients.AnyAsync(c => c.Id == clientId);

    public async Task<UserResponseDto?> GetByIdAsync(int clientId)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        return client != null ? mapper.MapToDto(client) : null;
    }

    public async Task<IEnumerable<UserResponseDto>> GetManyAsync(PaginationQuery pagination, string? search = null)
    {
        return await context.Clients
            .Where(c => (c.FirstName + " " + c.LastName).Contains(search ?? ""))
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1)).Take(pagination.LimitOrDefault)
            .Select(c => mapper.MapToDto(c)!)
            .ToListAsync();
    }

    public async Task<Result<UserResponseDto>> CreateAsync(UserCreateDto dto)
    {
        var client = mapper.MapToEntity(dto);

        var verification = await VerifyClient(client);
        if (!verification.IsSuccess)
            return verification.Error;

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        return mapper.MapToDto(client);
    }

    public async Task<Result<UserResponseDto>> UpdateAsync(int clientId, UserUpdateDto dto)
    {
        // TODO Rewrite UpdateAsync methods in all services
        var updated = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (updated == null)
            return new NotFoundError();

        mapper.ApplyUpdate(updated, dto);

        var verification = await VerifyClient(updated);
        if (!verification.IsSuccess)
        {
            await context.DisposeAsync();
            return verification.Error;
        }

        await context.SaveChangesAsync();

        return mapper.MapToDto(updated);
    }

    public async Task<UserResponseDto?> DeleteAsync(int clientId)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (client == null)
            return null;

        context.Clients.Remove(client);
        await context.SaveChangesAsync();

        return mapper.MapToDto(client);
    }

    private async Task<Result> VerifyClient(Client client)
    {
        if (!await EmailIsUnique(client))
            return new DuplicateEmailError(client.Email, client.Id > 0 ? client.Id : null);
        if (!await PhoneNumberIsUnique(client))
            return new DuplicatePhoneNumberError(client.PhoneNumber, client.Id > 0 ? client.Id : null);
        
        var validation = await validator.ValidateAsync(client);
        if (!validation.IsValid)
            return new ValidationError(validation.ToDictionary());
        
        return Result.Success();
    }

    private async Task<bool> EmailIsUnique(Client client)
        => !await context.Clients.AnyAsync(c => c.Email == client.Email && c != client);
    
    private async Task<bool> PhoneNumberIsUnique(Client client)
        => !await context.Clients.AnyAsync(c => c.PhoneNumber == client.PhoneNumber && c != client);
}

