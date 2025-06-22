using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class GameTag
{
    public int GameId { get; set; }
    public Game Game { get; set; }

    public int TagId { get; set; }
    public Tag Tag { get; set; }
}