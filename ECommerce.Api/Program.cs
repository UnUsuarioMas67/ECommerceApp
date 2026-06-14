using ECommerce.Api.EF;
using ECommerce.Api.EF.Seeding;
using ECommerce.Api.Extensions;
using ECommerce.Api.Settings;
using ECommerce.Api.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

builder.Services.AddDbContext<ECommerceContext>(options =>
{
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("ECommerceDb"))
        .AddSeedingToDbContext();
});

builder.Services.Configure<OrderExpirySettings>(builder.Configuration.GetSection("OrderExpirySettings"));
builder.Services.ConfigureAuth(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddStripeSettings(builder.Configuration.GetSection("Stripe"));
builder.Services.AddValidators();
builder.Services.AddObjectMappers();
builder.Services.AddApiServices();
builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.Kepler;
        options.AddPreferredSecuritySchemes(JwtBearerDefaults.AuthenticationScheme);
        options.AddHttpAuthentication(JwtBearerDefaults.AuthenticationScheme,
            o => o.Token =
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJ3d3cuZWNvbW1lcmNlLmNvbSIsImlzcyI6Ind3dy5lY29tbWVyY2UuY29tIiwiZXhwIjo0OTI4MDA3Mzg1LCJuYmYiOjE3NzIzMTkyNDIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJQYXRyaWNpbyBNYXJ0aW5leiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6InBtYXJ0aW5lekBlbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOlsiY2xpZW50IiwiYWRtaW4iXSwianRpIjoiNjg5YmVjMmEtZDZkNy00ZDczLTg4ZGQtNzMxNTI4YTlmMGUyIiwiaWF0IjoxNzcyMzE5MjQyfQ.jP-nFeOUfIqRc6KJe2Vtndj6RBxDKquWJnYJxOSCxew");
    });

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ECommerceContext>();
    if (dbContext.Database.GetPendingMigrations().Any())
        dbContext.Database.Migrate();
    else
        dbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.MapApiEndpoints();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();