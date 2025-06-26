using Microsoft.AspNetCore.Identity;
using MyGameShelf.Infrastructure.Identity;

namespace MyGameShelf.Web.Middleware;

public class LastActiveMiddleware
{
    private readonly RequestDelegate _next;

    public LastActiveMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user != null && user.LastActive.Date != DateTime.UtcNow.Date) // optional throttle
            {
                user.LastActive = DateTime.UtcNow;
                await userManager.UpdateAsync(user);
            }
        }

        await _next(context);
    }
}
