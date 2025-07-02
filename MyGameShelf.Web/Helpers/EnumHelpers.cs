using MyGameShelf.Domain.Enums;
using System.Numerics;

namespace MyGameShelf.Web.Helpers;

public static class EnumHelpers
{
    public static string GetGameStatusDisplay(GameStatus status)
    {
        return status switch
        {
            GameStatus.Playing => "Playing",
            GameStatus.Completed => "Completed",
            GameStatus.OnHold => "On Hold",
            GameStatus.Dropped => "Dropped",
            GameStatus.Planned => "Planned",
            GameStatus.Wishlist => "Wishlist",
            _ => status.ToString()
        };
    }
}
