using ECommerce.Api.Application.DTOs.Cart;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Errors;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.DataAccess;

public interface ICartsService
{
    Task<CartResponseDto?> GetByIdAsync(int cartId);
    Task<IEnumerable<CartResponseDto>> GetManyAsync(PaginationQuery pagination);
    Task<IEnumerable<CartResponseDto>> GetByClientAsync(int clientId, PaginationQuery pagination);
    Task<Result<CartResponseDto>> CreateAsync(CartRequestDto dto, int clientId);
    Task<Result<CartResponseDto>> UpdateAsync(int cartId, CartRequestDto dto, int? clientId = null);
    Task<CartResponseDto?> DeleteAsync(int cartId, int? clientId = null);
}

public class CartsService(ECommerceContext context, IValidator<Cart> validator, CartMapper mapper)
    : ICartsService
{
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
            .ThenInclude(product => product.Category)
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1)).Take(pagination.LimitOrDefault)
            .Select(item => mapper.MapToDto(item))
            .ToListAsync();
    }

    public async Task<IEnumerable<CartResponseDto>> GetByClientAsync(int clientId, PaginationQuery pagination)
    {
        return await context.Carts
            .Include(cart => cart.Items)
            .ThenInclude(item => item.Product)
            .ThenInclude(product => product.Category)
            .Where(c => c.ClientId == clientId)
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1)).Take(pagination.LimitOrDefault)
            .Select(item => mapper.MapToDto(item))
            .ToListAsync();
    }

    public async Task<Result<CartResponseDto>> CreateAsync(CartRequestDto dto, int clientId)
    {
        var created = await mapper.MapToEntityAsync(dto, clientId);

        var verification = await VerifyCart(created);
        if (!verification.IsSuccess)
            return verification.Error;

        await context.Carts.AddAsync(created);
        await context.SaveChangesAsync();

        return mapper.MapToDto(created);
    }

    public async Task<Result<CartResponseDto>> UpdateAsync(int cartId, CartRequestDto dto, int? clientId = null)
    {
        var query = context.Carts.Where(c => c.Id == cartId);

        if (clientId.HasValue)
            query = query.Where(c => c.ClientId == clientId.Value);

        var updated = await query.FirstOrDefaultAsync();
        if (updated == null)
            return new NotFoundError();

        await mapper.ApplyUpdateAsync(updated, dto);

        // Delete previous cart item entries
        await context.CartItems
            .Where(ci => ci.CartId == updated.Id)
            .ExecuteDeleteAsync();

        var verification = await VerifyCart(updated);
        if (!verification.IsSuccess)
        {
            await context.DisposeAsync();
            return verification.Error;
        }

        await context.SaveChangesAsync();

        return mapper.MapToDto(updated);
    }

    public async Task<CartResponseDto?> DeleteAsync(int cartId, int? clientId = null)
    {
        var query = context.Carts.Where(c => c.Id == cartId);

        if (clientId.HasValue)
            query = query.Where(c => c.ClientId == clientId.Value);

        var deleted = await query.FirstOrDefaultAsync();
        if (deleted == null)
            return null;

        context.Carts.Remove(deleted);
        await context.SaveChangesAsync();

        return mapper.MapToDto(deleted);
    }

    private async Task<Result> VerifyCart(Cart cart)
    {
        if (!await ClientExists(cart))
            return new ClientNotExistsError(cart.ClientId, cart.Id > 0 ? cart.Id : null);

        var invalidIds = await GetMissingProductIds(cart.Items);
        if (invalidIds.Length != 0)
            return new ProductsNotExistError(invalidIds);

        var duplicateIds = GetDuplicateProductIds(cart.Items);
        if (duplicateIds.Length != 0)
            return new DuplicateItemsError(duplicateIds);

        var invalidQuantitiesIds = GetProductIdsWithInvalidQuantities(cart.Items);
        if (invalidQuantitiesIds.Length != 0)
            return new InvalidQuantityError(invalidQuantitiesIds);
        
        var productsWithoutStock = cart.Items
            .Where(i => i.Product.Stock < i.Quantity)
            .Select(i => new ProductsStockErrorItem(i.Product.Id, i.Quantity, i.Product.Stock))
            .ToList();
        if (productsWithoutStock.Count != 0)
            return new ProductsStockError(productsWithoutStock);
        
        var validation = await validator.ValidateAsync(cart);
        if (!validation.IsValid)
            return new ValidationError(validation.ToDictionary());

        return Result.Success();
    }

    private async Task<bool> ClientExists(Cart cart)
        => await context.Clients.AnyAsync(c => c.Id == cart.ClientId || c == cart.Client);

    private async Task<int[]> GetMissingProductIds(ICollection<CartItem> items)
    {
        var productIds = items.Select(i => i.ProductId).ToList();
        var idsOnDb = await context.Products
            .AsNoTracking()
            .Where(p => productIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        var idsNotOnDb = productIds.Except(idsOnDb);
        return idsNotOnDb.ToArray();
    }

    private int[] GetDuplicateProductIds(ICollection<CartItem> items)
        => items
            .GroupBy(i => i.ProductId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();


    private int[] GetProductIdsWithInvalidQuantities(ICollection<CartItem> items)
        => items
            .Where(i => i.Quantity <= 0)
            .Select(i => i.ProductId)
            .ToArray();
}