using Microsoft.AspNetCore.Identity;
using MyGameShelf.Infrastructure.Identity;

namespace MyGameShelf.Web.Helpers;

public static class UserHelpers
{
    public static async Task<string> GenerateUniqueUsername(UserManager<ApplicationUser> userManager, string baseUsername)
    {
        string username = baseUsername;
        int suffix = 1;

        while (await userManager.FindByNameAsync(username) != null)
        {
            username = $"{baseUsername}{suffix}";
            suffix++;
        }

        return username;
    }
}