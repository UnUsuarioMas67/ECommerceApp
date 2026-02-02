using System.Text;
using ECommerce.Api.Application.Auth;
using ECommerce.Api.Infrastructure.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ECommerceContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("ECommerceDb")));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

        o.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.SecretKey)),
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
    });
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy(UserRoles.Client, policy => policy.RequireRole(UserRoles.Client, UserRoles.Admin));
    o.AddPolicy(UserRoles.Admin, policy => policy.RequireRole(UserRoles.Admin));
});

// builder.Services.AddSwaggerGenWithAuth();

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

app.Run();