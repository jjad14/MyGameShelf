using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.DTOs;
public class GameDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime? Released { get; set; }
    public string BackgroundImage { get; set; }
    public double? Rating { get; set; }
    public int? Metacritic { get; set; }
    public IEnumerable<string> Genres { get; set; } 
    public IEnumerable<string> Tags { get; set; } 
    public IEnumerable<string> Platforms { get; set; }

    // Filters to preserve
    public string? Search { get; set; }
    public string? Platform { get; set; }
    public string? Genre { get; set; }
    public string? MetacriticFilter { get; set; }
    public string? OrderBy { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
