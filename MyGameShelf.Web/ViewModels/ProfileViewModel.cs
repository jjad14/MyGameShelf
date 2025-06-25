using MyGameShelf.Domain.Models;

namespace MyGameShelf.Web.ViewModels;

public class ProfileViewModel
{
    public string Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? ProfileMessage { get; set; }
    public string? Gender { get; set; }
    public DateTime Birthday { get; set; }
    public Address? Address { get; set; }
    public string? XSocialLink { get; set; }
    public string? InstagramSocialLink { get; set; }
    public string? FacebookSocialLink { get; set; }
    public string? YoutubeSocialLink { get; set; }
    public string? TwitchSocialLink { get; set; }
}
