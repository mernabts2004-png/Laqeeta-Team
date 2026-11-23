using Laqeeta.Web.Configuration;
using Laqeeta.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.Configure<LaqeetaOptions>(
    builder.Configuration.GetSection(LaqeetaOptions.SectionName));

builder.Services.AddHttpContextAccessor();

// Typed HttpClient that talks to Laqeeta.API.
var isDevelopment = builder.Environment.IsDevelopment();

builder.Services
    .AddHttpClient<LaqeetaApiClient>((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<LaqeetaOptions>>().Value;

        client.BaseAddress = new Uri(options.ApiBaseUrl.TrimEnd('/') + "/");
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();

        // In development only: accept the local dev certificate of the API.
        if (isDevelopment)
        {
            handler.ServerCertificateCustomValidationCallback =
                (message, _, _, _) => message.RequestUri?.IsLoopback == true;
        }

        return handler;
    });

// The JWT returned by the API is kept inside the login cookie.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "Laqeeta.Auth";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = false;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
