using ECommerce.Api.Application.DTOs.Cart;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.DataAccess;

public interface ICartsService
{
    Task<bool> EntryExistsAsync(int cartId);
    Task<CartResponseDto?> GetByIdAsync(int cartId);
    Task<IEnumerable<CartResponseDto>> GetManyAsync(PaginationQuery pagination);
    Task<IEnumerable<CartResponseDto>> GetByClientAsync(int clientId, PaginationQuery pagination);
    Task<Result<CartResponseDto>> CreateAsync(CartCreateDto dto);
    Task<Result<CartResponseDto>> UpdateAsync(int cartId, CartUpdateDto dto);
    Task<CartResponseDto?> DeleteAsync(int cartId);
}

public class CartsService(ECommerceContext context, IValidator<Cart> validator, CartMapper mapper)
    : ICartsService
{
    public async Task<bool> EntryExistsAsync(int cartId)
        => await context.Carts.AnyAsync(c => c.Id == cartId);

    public async Task<CartResponseDto?> GetByIdAsync(int cartId)
    {
        var cart = await context.Carts
            .Include(cart => cart.Items)
            .ThenInclude(item => item.Product)
            .ThenInclude(product => product.Category)
            .FirstOrDefaultAsync(c => c.Id == cartId);
        return cart != null ? mapper.MapToDto(cart) : null;
    }

    public async Task<IEnumerable<CartResponseDto>> GetManyAsync(PaginationQuery pagination)
    {
        return await context.Carts
            .Include(cart => cart.Items)
            .ThenInclude(item => item.Product)
            .ThenInclude(product => product!.Category)
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1)).Take(pagination.LimitOrDefault)
            .Select(item => mapper.MapToDto(item))
            .ToListAsync();
    }

    public async Task<IEnumerable<CartResponseDto>> GetByClientAsync(int clientId, PaginationQuery pagination)
    {
        return await context.Carts
            .Include(cart => cart.Items)
            .ThenInclude(item => item.Product)
            .ThenInclude(product => product!.Category)
            .Where(c => c.ClientId == clientId)
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1)).Take(pagination.LimitOrDefault)
            .Select(item => mapper.MapToDto(item))
            .ToListAsync();
    }

    public async Task<Result<CartResponseDto>> CreateAsync(CartCreateDto dto)
    {
        var created = await mapper.MapToEntityAsync(dto);

        var validationResult = await Validate(created);
        if (!validationResult.IsSuccess)
            return Errors.ValidationError(validationResult.Error!.Details);

        await context.Carts.AddAsync(created);
        await context.SaveChangesAsync();

        return mapper.MapToDto(created);
    }

    public async Task<Result<CartResponseDto>> UpdateAsync(int cartId, CartUpdateDto dto)
    {
        var updated = await context.Carts.FindAsync(cartId);
        if (updated == null)
            return Errors.NotFound();

        await mapper.ApplyUpdateAsync(updated, dto);

        // Delete previous cart item entries
        await context.CartItems
            .Where(ci => ci.CartId == updated.Id)
            .ExecuteDeleteAsync();

        var validationResult = await Validate(updated);
        if (!validationResult.IsSuccess)
        {
            await context.DisposeAsync();
            return Errors.ValidationError(validationResult.Error!.Details);
        }

        await context.SaveChangesAsync();

        return mapper.MapToDto(updated);
    }

    public async Task<CartResponseDto?> DeleteAsync(int cartId)
    {
        var deleted = await context.Carts.FindAsync(cartId);
        if (deleted == null)
            return null;

        context.Carts.Remove(deleted);
        await context.SaveChangesAsync();

        return mapper.MapToDto(deleted);
    }

    private async Task<Result> Validate(Cart cart)
    {
        var validation = await validator.ValidateAsync(cart);
        if (!validation.IsValid)
            return Errors.ValidationError(validation.ToDictionary());

        var invalidItems = await GetInvalidCartItems(cart.Items);

        if (invalidItems.Length == 0)
            return Result.Success();
        
        var errorDetails = new Dictionary<string, string[]>();
        var ids = string.Join(", ", invalidItems);
        errorDetails.Add(nameof(cart.Items), [$"Could not find the following product id(s): {ids}"]);
        
        return Errors.ValidationError(errorDetails);
    }

    private async Task<bool> ClientExists(int clientId)
        => await context.Clients.AnyAsync(c => c.Id == clientId);

    private async Task<int[]> GetInvalidCartItems(ICollection<CartItem> items)
    {
        var itemProductIds = items.Select(item => item.ProductId).Distinct().ToList();
        var dbProductIds = await context.Products
            .Where(p => itemProductIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        return itemProductIds.Except(dbProductIds).ToArray();
    }
}