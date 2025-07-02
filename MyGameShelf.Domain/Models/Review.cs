using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Domain.Models;
public class Review
{
    [Key]
    public int Id { get; set; }

    public string UserId { get; set; }

    [ForeignKey("Game")]
    public int GameId { get; set; }
    public Game Game { get; set; }

    private string _content = null!;
    public string Content
    {
        get => _content;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Review content cannot be empty.", nameof(Content));
            if (value.Length > 1000)
                throw new ArgumentException("Review content cannot exceed 1000 characters.", nameof(Content));
            _content = value;
        }
    }

    public bool? IsRecommended { get; set; }

    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    private Review() { }

    public Review(string userId, int gameId, string content, bool? recommended)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        GameId = gameId;
        Content = content;
        IsRecommended = recommended;    
    }

    public void UpdateReview(string newContent, bool newRecommended)
    {
        // Use property setters to validate inputs
        Content = newContent;
        IsRecommended = newRecommended;

        UpdatedAt = DateTime.UtcNow;
    }
}
