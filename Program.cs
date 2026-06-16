using AptekaDiplom2.Components;
using AptekaDiplom2.Data;
using AptekaDiplom2.Services;
using AptekaDiplom2.State;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<CartState>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPharmacyService, PharmacyService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddMemoryCache();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "AptekaDiplom2.Auth";
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

//Эндпоинты для входа/регистрации/выхода
app.MapPost("/api/auth/login", async (HttpContext httpContext, IAuthService authService) =>
{
    var form = await httpContext.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();
    var returnUrl = form["returnUrl"].ToString();
    if (string.IsNullOrWhiteSpace(returnUrl)) returnUrl = "/";

    var result = await authService.LoginAsync(email, password);
    if (!result.Success || result.User == null)
    {
        return Results.Redirect($"/login?error={Uri.EscapeDataString(result.Message)}&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    var claims = new List<System.Security.Claims.Claim>
    {
        new(System.Security.Claims.ClaimTypes.NameIdentifier, result.User.Id.ToString()),
        new(System.Security.Claims.ClaimTypes.Email, result.User.Email),
        new(System.Security.Claims.ClaimTypes.Name, result.User.FullName ?? result.User.Email),
        new(System.Security.Claims.ClaimTypes.Role, result.User.Role)
    };
    var identity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new System.Security.Claims.ClaimsPrincipal(identity);

    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    return Results.Redirect(returnUrl);
});

app.MapPost("/api/auth/register", async (HttpContext httpContext, IAuthService authService) =>
{
    var form = await httpContext.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();
    var confirmPassword = form["confirmPassword"].ToString();
    var fullName = form["fullName"].ToString();
    var phone = form["phone"].ToString();
    var returnUrl = form["returnUrl"].ToString();
    if (string.IsNullOrWhiteSpace(returnUrl)) returnUrl = "/";

    if (password != confirmPassword)
    {
        return Results.Redirect($"/register?error={Uri.EscapeDataString("Пароли не совпадают.")}&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    var result = await authService.RegisterAsync(email, password, fullName, phone);
    if (!result.Success || result.User == null)
    {
        return Results.Redirect($"/register?error={Uri.EscapeDataString(result.Message)}&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    var claims = new List<System.Security.Claims.Claim>
    {
        new(System.Security.Claims.ClaimTypes.NameIdentifier, result.User.Id.ToString()),
        new(System.Security.Claims.ClaimTypes.Email, result.User.Email),
        new(System.Security.Claims.ClaimTypes.Name, result.User.FullName ?? result.User.Email),
        new(System.Security.Claims.ClaimTypes.Role, result.User.Role)
    };
    var identity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new System.Security.Claims.ClaimsPrincipal(identity);

    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    return Results.Redirect(returnUrl);
});

app.MapPost("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AptekaDiplom2.Data.ApplicationDbContext>();
        
        var adminEmail = "admin@admin.ru";
        
        //Вызов Any через стандартный LINQ
        var adminExists = System.Linq.Queryable.Any(context.Users, u => u.Email == adminEmail);

        if (!adminExists)
        {
            var adminUser = new AptekaDiplom2.Models.User
            {
                Email = adminEmail,
                PasswordHash = "admin", 
                FullName = "Главный Администратор",
                Phone = "+79990000000",
                Role = "Admin"
            };

            context.Users.Add(adminUser);
            context.SaveChanges();
            Console.WriteLine("--> [УСПЕХ] Администратор admin@admin.ru успешно создан в базе данных!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> [ОШИБКА] Не удалось создать админа при старте: {ex.Message}");
    }
}

app.Run();