using ECommerce.Api.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ECommerceContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("ECommerceDb")));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ECommerceContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapGet("/test", (ECommerceContext dbContext) => dbContext.OrderStatuses.ToList());

app.Run();