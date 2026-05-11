using ECommerce.Dashboard.DelegateHandlers;
using ECommerce.Dashboard.Middleware;
using ECommerce.Dashboard.Services;
using ECommerce.Dashboard.Services.Api;
using ECommerce.Dashboard.Settings;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", o =>
    {
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        
        o.LoginPath = "/Account/Login";
        o.LogoutPath = "/Account/Logout";
    });

builder.Services.AddAuthorization();

builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<ApiAuthorizationHeaderHandler>();
builder.Services.AddHttpClient("ApiClient" ,(serviceProvider, client) =>
{
    var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(apiSettings.ApiUrl);
}).AddHttpMessageHandler<ApiAuthorizationHeaderHandler>();

builder.Services.AddSingleton<CookieHelperService>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ApiRequestService>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseMiddleware<ApiTokenRefreshHandler>();

app.Run();