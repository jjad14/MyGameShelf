using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Enums;
public enum GameStatus
{
    Playing, // default status - enums default to 0
    Completed,
    OnHold,
    Dropped,
    Planned,
    Wishlist
}

public static class GameStatusExtensions
{
    public static string ToLabel(this GameStatus status) => status switch
    {
        GameStatus.Playing => "Playing",
        GameStatus.Completed => "Completed",
        GameStatus.OnHold => "On Hold",
        GameStatus.Dropped => "Dropped",
        GameStatus.Planned => "Planned",
        GameStatus.Wishlist => "Wishlist",
        _ => "Unknown"
    };
}