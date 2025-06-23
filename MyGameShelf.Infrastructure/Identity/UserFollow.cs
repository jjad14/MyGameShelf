using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.Identity;
public class UserFollow
{
    [ForeignKey(nameof(Follower))]
    public string FollowerId { get; set; }
    public ApplicationUser Follower { get; set; }

    [ForeignKey(nameof(Followee))]
    public string FolloweeId { get; set; }
    public ApplicationUser Followee { get; set; }

    public DateTime FollowedOn { get; set; } = DateTime.UtcNow;
}