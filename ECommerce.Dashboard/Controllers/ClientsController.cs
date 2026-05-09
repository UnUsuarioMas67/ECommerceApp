using ECommerce.Dashboard.DTOs.Shared;
using ECommerce.Dashboard.Filters;
using ECommerce.Dashboard.Models.Clients;
using ECommerce.Dashboard.Services.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Dashboard.Controllers;

[Authorize]
[TypeFilter<HandleApi401Exception>]
public class ClientsController(ClientService clientService) : Controller
{
    public async Task<IActionResult> Index(int page = 1, string? searchTerm = null)
    {
        var clients = await clientService.GetClients(searchTerm, new PaginationQuery(20, page));

        var viewModel = new ClientListViewModel
        {
            Clients = clients.ToList(),
            SearchTerm = searchTerm
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await clientService.GetClientById(id);
        if (result.IsSuccess)
            return View(result.Value);

        return RedirectToAction(nameof(NotFound));
    }

    [ActionName("NotFound")]
    public IActionResult ClientNotFound()
    {
        return View();
    }
}