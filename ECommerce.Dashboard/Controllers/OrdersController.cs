using ECommerce.Dashboard.DTOs.Shared;
using ECommerce.Dashboard.Filters;
using ECommerce.Dashboard.Models.Orders;
using ECommerce.Dashboard.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Dashboard.Controllers;

[Authorize]
[TypeFilter<HandleApi401Exception>]
public class OrdersController(OrderService orderService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string? status = null)
    {
        var orders = await orderService.GetOrders(new PaginationQuery(20, page));
        
        var orderList = status != null ? orders.Where(o => o.Status == status).ToList() : orders.ToList();

        var viewModel = new OrderListViewModel
        {
            Orders = orderList,
            Status = status,
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await orderService.GetOrderById(id);
        if (result.IsSuccess)
            return View(result.Value);

        return RedirectToAction(nameof(NotFound));
    }

    [ActionName("NotFound")]
    public IActionResult OrderNotFound()
    {
        return View();
    }
}