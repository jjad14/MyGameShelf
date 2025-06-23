using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class DeveloperFollow
{
    public int Id { get; set; }
    public int DeveloperId { get; set; }
    public Developer Developer { get; set; }

    public string UserId { get; set; } // Foreign key to ApplicationUser (infrastructure)

    public DateTime FollowedOn { get; set; } = DateTime.UtcNow;
}