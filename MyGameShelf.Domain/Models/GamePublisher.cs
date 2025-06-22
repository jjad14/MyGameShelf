using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class GamePublisher
{
    public int GameId { get; set; }
    public Game Game { get; set; }

    public int PublisherId { get; set; }
    public Publisher Publisher { get; set; }
}
