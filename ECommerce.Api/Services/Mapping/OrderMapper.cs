using ECommerce.Api.DTOs.Order;
using ECommerce.Api.Entities;

namespace ECommerce.Api.Services.Mapping;

public class OrderMapper(AddressMapper addressMapper)
{
    public OrderResponseDto MapToDto(ShopOrder order)
    {
        var dto = new OrderResponseDto
        {
            Id = order.Id,
            ClientId = order.ClientId,
            ClientEmail = order.Client?.Email,
            ClientName = order.Client != null ? order.Client.FirstName + " " + order.Client.LastName : null,
            Address = addressMapper.MapToDto(order.Address),
            OrderDate = order.OrderDate,
            Status = OrderStatuses.GetStatusName(order.StatusId),
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
            ProductId = line.ProductId,
            ProductName = line.Product.Name,
            ProductSku = line.Product.Sku,
            Quantity = line.Quantity,
            UnitPrice = line.UnitPrice,
            Subtotal = line.UnitPrice * line.Quantity
        };
    }
}
