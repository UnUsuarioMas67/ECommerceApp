using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
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

public class ClientsService(ECommerceContext context, IValidator<Client> validator, IClientMapper mapper)
    : IClientsService
{
    public async Task<bool> EntryExistsAsync(int clientId)
        => await context.Clients.AnyAsync(c => c.Id == clientId);

    public async Task<UserResponseDto?> GetByIdAsync(int clientId)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        return client != null ? mapper.ToDto(client) : null;
    }

    public async Task<IEnumerable<UserResponseDto>> GetManyAsync(PaginationQuery pagination, string? search = null)
    {
        return await context.Clients
            .Where(c => (c.FirstName + " " + c.LastName).Contains(search ?? ""))
            .Skip(pagination.Skip ?? PaginationDefaults.Skip).Take(pagination.Limit ?? PaginationDefaults.Limit)
            .Select(c => mapper.ToDto(c)!)
            .ToListAsync();
    }

    public async Task<Result<UserResponseDto>> CreateAsync(UserCreateDto dto)
    {
        var client = mapper.ToEntity(dto);

        var validationResult = await Validate(client);
        if (!validationResult.IsSuccess)
            return Errors.ValidationError(validationResult.Error!.Details);

        await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();

        return mapper.ToDto(client);
    }

    public async Task<Result<UserResponseDto>> UpdateAsync(int clientId, UserUpdateDto dto)
    {
        // TODO Rewrite UpdateAsync methods in all services
        var updated = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (updated == null)
            return Errors.NotFound();

        InnerUpdate(updated, dto);

        var validationResult = await Validate(updated);
        if (!validationResult.IsSuccess)
        {
            await context.DisposeAsync();
            return Errors.ValidationError(validationResult.Error!.Details);
        }

        await context.SaveChangesAsync();

        return mapper.ToDto(updated);
    }

    public async Task<UserResponseDto?> DeleteAsync(int clientId)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (client == null)
            return null;

        context.Clients.Remove(client);
        await context.SaveChangesAsync();

        return mapper.ToDto(client);
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

    private void InnerUpdate(Client toUpdate, UserUpdateDto dto)
    {
        if (dto.FirstName != null && dto.FirstName != toUpdate.FirstName)
            toUpdate.FirstName = dto.FirstName;

        if (dto.LastName != null && dto.LastName != toUpdate.LastName)
            toUpdate.LastName = dto.LastName;

        if (dto.PhoneNumber != null && dto.PhoneNumber != toUpdate.PhoneNumber)
            toUpdate.PhoneNumber = dto.PhoneNumber;

        if (DateOnly.TryParseExact(dto.BirthDate, "yyyy-MM-dd", out var birthDate) && birthDate != toUpdate.BirthDate)
            toUpdate.BirthDate = birthDate;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            if (newPasswordHash != toUpdate.PasswordHash)
                toUpdate.PasswordHash = newPasswordHash;
        }
    }
}