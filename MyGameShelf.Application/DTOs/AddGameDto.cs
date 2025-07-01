using MyGameShelf.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.DTOs;
public class AddGameDto
{
    public int GameId { get; set; }
    public string GameStatus { get; set; }
    public int Rating { get; set; }
    public int Difficulty { get; set; }
    public string Review { get; set; }

}
