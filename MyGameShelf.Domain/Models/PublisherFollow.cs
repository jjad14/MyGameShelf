using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class PublisherFollow
{
    public int Id { get; set; }
    public int PublisherId { get; set; }
    public Publisher Publisher { get; set; }

    public string UserId { get; set; } // Foreign key to ApplicationUser

    public DateTime FollowedOn { get; set; } = DateTime.UtcNow;
}