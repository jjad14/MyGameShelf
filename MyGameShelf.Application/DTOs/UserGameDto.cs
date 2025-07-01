using MyGameShelf.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.DTOs;
public class UserGameDto
{
    public int GameId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? BackgroundImage { get; set; }
    public GameStatus Status { get; set; }
    public double? Difficulty { get; set; }
    public DateTime AddedOn { get; set; }

    public List<string> Genres { get; set; } = new();
    public List<string> Platforms { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}
