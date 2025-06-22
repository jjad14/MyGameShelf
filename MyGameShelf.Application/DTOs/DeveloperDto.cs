using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.DTOs;
public class DeveloperDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public int GamesCount { get; set; }
    public string ImageBackground { get; set; }
}
