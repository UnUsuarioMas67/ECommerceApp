using System.Text.Json;
using ECommerce.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.EF.Seeding;

public class ClientSeed(string filePath) : Seed<Client>(filePath)
{
    protected override Client[] LoadFromJson()
    {
        var clients = base.LoadFromJson();
        return clients.Select(client =>
        {
            client.PhoneNumber = client.PhoneNumber.Replace(" ", string.Empty);
            return client;
        }).ToArray();
    }
}