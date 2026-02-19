using ECommerce.Api.Extensions;
using ECommerce.Api.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ECommerceContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("ECommerceDb")));

builder.Services.ConfigureAuth(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddValidators();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ECommerceContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/test", (ECommerceContext dbContext) => dbContext.OrderStatuses.ToList());

app.MapApiEndpoints();

app.Run();