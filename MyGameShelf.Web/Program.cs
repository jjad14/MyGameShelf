using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyGameShelf.Application.Configurations;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Infrastructure.Data;
using MyGameShelf.Infrastructure.Identity;
using MyGameShelf.Infrastructure.Services;
using MyGameShelf.Web.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Rawg API Key
builder.Services.Configure<RawgSettings>(builder.Configuration.GetSection("RawgSettings"));
builder.Services.AddHttpClient<IRawgApiService, RawgApiService>();

// Cloudinary
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));


// Add DbContext to the DI container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// AddMemoryCache is used to cache data in memory, which can improve performance by reducing database calls.
builder.Services.AddMemoryCache();

// AddSession is used to manage user sessions, allowing you to store user-specific data across requests.
// This means you can keep track of user data like login status, preferences, etc., during their session.
builder.Services.AddSession();

// Configure authentication to use cookies
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login"; // Path for login page
    options.LogoutPath = "/Auth/Logout";
    //options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "MyGameShelfAuthCookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
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

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
