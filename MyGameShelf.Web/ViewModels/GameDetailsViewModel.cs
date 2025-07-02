using Microsoft.AspNetCore.Mvc.Rendering;
using MyGameShelf.Application.DTOs;

namespace MyGameShelf.Web.ViewModels;

public class GameDetailsViewModel
{
    public GameDetailDto Game { get; set; }

    public string? PublisherIdsString { get; set; }
    public bool HasRelatedGames { get; set; }
    public bool HasSequels { get; set; }
    public bool HasAdditions { get; set; }

    public AddGameToListViewModel? AddToList { get; set; }


    public List<SelectListItem> GameStatusOptions { get; set; } = new()
    {
        new SelectListItem { Text = "Playing", Value = "Playing" },
        new SelectListItem { Text = "Completed", Value = "Completed" },
        new SelectListItem { Text = "On Hold", Value = "OnHold" },
        new SelectListItem { Text = "Dropped", Value = "Dropped" },
        new SelectListItem { Text = "Plan to Play", Value = "Planned" },
        new SelectListItem { Text = "Wishlist", Value = "Wishlist" },
    };

    public List<SelectListItem> RatingOptions { get; set; } = Enumerable.Range(1, 10)
    .Reverse()
    .Select(i => new SelectListItem
    {
        Value = i.ToString(),
        Text = i switch
        {
            10 => "10 - Perfect",
            9 => "9 - Great",
            8 => "8 - Very Good",
            7 => "7 - Good",
            6 => "6 - Above Average",
            5 => "5 - Average",
            4 => "4 - Below Average",
            3 => "3 - Poor",
            2 => "2 - Very Poor",
            1 => "1 - Unacceptable",
            _ => i.ToString()
        }
    }).ToList();

    public List<SelectListItem> DifficultyOptions { get; set; } = Enumerable.Range(1, 10)
    .Reverse()
    .Select(i => new SelectListItem
    {
        Value = i.ToString(),
        Text = i switch
        {
            10 => "10 - Extremely Hard",
            9 => "9 - Very Hard",
            8 => "8 - Hard",
            7 => "7 - Challenging",
            6 => "6 - Moderately Hard",
            5 => "5 - Medium",
            4 => "4 - Fairly Easy",
            3 => "3 - Easy",
            2 => "2 - Very Easy",
            1 => "1 - Extremely Easy",
            _ => i.ToString()
        }
    }).ToList();
}

