using ECommerce.Api.Application.DTOs.Order;
using ECommerce.Api.Entities;

namespace ECommerce.Api.Services.Mapping;

public class OrderMapper
{
    public OrderResponseDto MapToDto(ShopOrder order)
    {
        var dto = new OrderResponseDto
        {
            Id = order.Id,
            ClientId = order.ClientId,
            AddressId = order.AddressId,
            OrderDate = order.OrderDate,
            Items = order.Items.Select(MapLineToDto).ToList()
        };

        dto.TotalPrice = dto.Items.Sum(i => i.Subtotal);
        dto.TotalProducts = dto.Items.Sum(i => i.Quantity);

        return dto;
    }

    private OrderLineResponseDto MapLineToDto(OrderLine line)
    {
        return new OrderLineResponseDto
        {
            Id = line.Id,
            ProductId = line.ProductId,
            ProductName = line.Product.Name,
            ProductSku = line.Product.Sku,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice,
            Subtotal = line.UnitPrice * line.Quantity
        };
    }
}
