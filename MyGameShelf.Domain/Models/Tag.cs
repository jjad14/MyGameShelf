using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }

    // This connects the tag to the many games it is associated with
    public ICollection<GameTag> GameTags { get; set; }
}