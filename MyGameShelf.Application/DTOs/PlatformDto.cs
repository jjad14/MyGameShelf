using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyGameShelf.Application.DTOs;
public class PlatformDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public int GamesCount { get; set; }
    public string ImageBackground { get; set; }
    public int? YearStart { get; set; }
    public int? YearEnd { get; set; }
}
