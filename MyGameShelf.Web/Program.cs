using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyGameShelf.Application.Configurations;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Application.Services;
using MyGameShelf.Infrastructure.Data;
using MyGameShelf.Infrastructure.Identity;
using MyGameShelf.Infrastructure.Repositories;
using MyGameShelf.Infrastructure.Services;
using MyGameShelf.Web.Helpers;
using MyGameShelf.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext to the DI container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IGameService, GameService>();

// Rawg API Key
builder.Services.Configure<RawgSettings>(builder.Configuration.GetSection("RawgSettings"));
builder.Services.AddHttpClient<IRawgApiService, RawgApiService>();

// Cloudinary
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSingleton<Cloudinary>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;

    var account = new Account(
        settings.CloudName,
        settings.ApiKey,
        settings.ApiSecret
    );

    return new Cloudinary(account);
});
builder.Services.AddScoped<IPhotoService, PhotoService>();

// Add Identity
// Add Identity with default UI and token providers
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // You can configure password, lockout, user settings here if needed
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredUniqueChars = 1;
        options.Password.RequiredLength = 8;
        options.SignIn.RequireConfirmedAccount = false; // email confirmation

    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// AddMemoryCache is used to cache data in memory, which can improve performance by reducing database calls.
builder.Services.AddMemoryCache();

// AddSession is used to manage user sessions, allowing you to store user-specific data across requests.
// This means you can keep track of user data like login status, preferences, etc., during their session.
builder.Services.AddSession();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login"; // Path for login page
    options.LogoutPath = "/Auth/Logout";
    //options.AccessDeniedPath = "/Auth/AccessDenied";
    options.Cookie.Name = "MyGameShelfAuthCookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

// Mapping Google Auth Keys to GoogleAuthSettings
var googleAuthSettings = builder.Configuration
    .GetSection("Authentication:Google")
    .Get<GoogleAuthSettings>();

// Adding Google Authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = googleAuthSettings.ClientId;
        options.ClientSecret = googleAuthSettings.ClientSecret;
    });



var app = builder.Build();

// Add roles 
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = new[] { UserRoles.Admin, UserRoles.User };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Cache common rawg api calls
using (var scope = app.Services.CreateScope())
{
    var rawgService = scope.ServiceProvider.GetRequiredService<IRawgApiService>();
    await rawgService.WarmUpPopularGameCache();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseMiddleware<LastActiveMiddleware>();

app.UseAuthorization();

// Catch all unhandled 404s:
app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/Error/NotFound");
    }
});

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
