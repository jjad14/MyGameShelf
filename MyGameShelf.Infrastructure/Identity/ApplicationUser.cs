using Microsoft.AspNetCore.Identity;
using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? ProfilePicturePublicId { get; set; }
    public string? ProfilePictureUrl { get; set; }

    public string? ProfileMessage { get; set; }
    public string Gender { get; set; }
    public DateTime Birthday { get; set; }
    public Address Address { get; set; } 

    public bool IsPublic { get; set; }

    public string? XSocialLink { get; set; }
    public string? InstagramSocialLink { get; set; }
    public string? FacebookSocialLink { get; set; }
    public string? YoutubeSocialLink { get; set; }
    public string? TwitchSocialLink { get; set; }

    public ICollection<UserGame> UserGames { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<Favorite> Favorites { get; set; }


    // Users this user is following
    public ICollection<UserFollow> Following { get; set; }

    // Users who are following this user
    public ICollection<UserFollow> Followers { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime LastActive { get; set; }
}
