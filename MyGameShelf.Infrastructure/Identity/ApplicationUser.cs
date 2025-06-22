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
    public string ProfilePictureUrl { get; set; }

    public string ProfileMessage { get; set; }
    public string Gender { get; set; }
    public DateTime Birthday { get; set; }
    public Address Address { get; set; } 

    public bool IsPublic { get; set; }

    public string XSocialLink { get; set; }
    public string InstagramSocialLink { get; set; }
    public string FacebookSocialLink { get; set; }
    public string YoutubeSocialLink { get; set; }
    public string TwitchSocialLink { get; set; }

    public ICollection<UserGame> UserGames { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<Favorite> Favorites { get; set; }
}
