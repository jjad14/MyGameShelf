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

builder.Services.AddScoped<IUserGameRepository, UserGameRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IDeveloperRepository, DeveloperRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ITagsRepository, TagRepository>();

builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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

// Add Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"];
    options.InstanceName = "MyGameShelf_";
});

// AddMemoryCache is used to cache data in memory, which can improve performance by reducing database calls.
builder.Services.AddMemoryCache();

// AddSession is used to manage user sessions, allowing you to store user-specific data across requests.
// This means you can keep track of user data like login status, preferences, etc., during their session.
builder.Services.AddSession();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login"; // Path for login page
    options.LogoutPath = "/Auth/Logout";
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
//using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // auto cancel after 30s
using (var scope = app.Services.CreateScope())
{
    var rawgService = scope.ServiceProvider.GetRequiredService<IRawgApiService>();
    await rawgService.WarmUpPopularGameCache();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Internal");
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
    var response = context.HttpContext.Response;

    switch (response.StatusCode)
    {
        case 404:
            response.Redirect("/Error/NotFound");
            break;
        case 403:
            response.Redirect("/Error/Forbidden");
            break;
        case 401:
            response.Redirect("/Error/Unauthorized");
            break;
    }
});

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
