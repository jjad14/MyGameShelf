using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class Developer
{
    public int Id { get; set; }
    public string Name { get; set; }

    // This connects the developer to the many games they have worked on
    public ICollection<GameDeveloper> GameDevelopers { get; set; }
}
