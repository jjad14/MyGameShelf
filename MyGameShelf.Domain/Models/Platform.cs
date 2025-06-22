using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class Platform
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    // This connects the platform to the many games it's used in
    public ICollection<GamePlatform> GamePlatforms { get; set; }
}
