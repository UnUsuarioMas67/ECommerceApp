using System.Text;
using ECommerce.Api.Services.Auth;
using ECommerce.Api.Services.Checkout;
using ECommerce.Api.Services.DataAccess;
using ECommerce.Api.Services.Mapping;
using ECommerce.Api.Settings;
using ECommerce.Api.Shared;
using ECommerce.Api.Validators.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;
using ProductService = ECommerce.Api.Services.DataAccess.ProductService;

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
        services.AddScoped<JwtService>();
        services.AddScoped<AuthenticationService>();

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
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IStripeCheckoutService, StripeCheckoutService>();

        services.AddScoped<SessionService>();
        services.AddHostedService<BackgroundOrderExpiryManager>();
    }

    public static void AddStripeSettings(this IServiceCollection services, IConfiguration stripeConfiguration)
    {
        services.Configure<StripeSettings>(stripeConfiguration);
        StripeConfiguration.ApiKey = stripeConfiguration.Get<StripeSettings>()?.SecretKey 
                                     ?? throw new ArgumentNullException(nameof(stripeConfiguration));
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