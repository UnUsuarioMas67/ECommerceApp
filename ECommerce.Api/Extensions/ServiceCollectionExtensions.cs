using System.Text;
using ECommerce.Api.Application.Services.Auth;
using ECommerce.Api.Application.Services.DataAccess;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.Application.Services.Payment;
using ECommerce.Api.Settings;
using ECommerce.Api.Shared;
using ECommerce.Api.Validators.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureAuth(this IServiceCollection services,
        IConfiguration jwtConfiguration)
    {
        services.Configure<JwtSettings>(jwtConfiguration);
        services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                var jwtSettings = jwtConfiguration.Get<JwtSettings>();

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
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddAuthorization(o =>
        {
            o.AddPolicy(UserRoles.Client, policy => policy.RequireRole(UserRoles.Client));
            o.AddPolicy(UserRoles.Admin, policy => policy.RequireRole(UserRoles.Admin));
        });
    }

    public static void AddValidators(this IServiceCollection services)
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;
        services.AddValidatorsFromAssemblyContaining<ProductValidator>();
    }

    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IClientsService, ClientsService>();
        services.AddScoped<IAdminsService, AdminsService>();
        services.AddScoped<IAddressesService, AddressesService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartsService, CartsService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderBuilderService, OrderBuilderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IPaymentBuilderService, PaymentBuilderService>();
        services.AddScoped<IStripeCheckoutService, StripeCheckoutService>();
    }

    public static void AddStripeSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
    }

    public static void AddObjectMappers(this IServiceCollection services)
    {
        services.AddScoped<ClientMapper>();
        services.AddScoped<AdminMapper>();
        services.AddScoped<AddressMapper>();
        services.AddScoped<CategoryMapper>();
        services.AddScoped<ProductMapper>();
        services.AddScoped<CartMapper>();
        services.AddScoped<CartItemMapper>();
        services.AddScoped<OrderMapper>();
    }
}