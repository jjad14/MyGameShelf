using System.ComponentModel.DataAnnotations;

namespace MyGameShelf.Web.ViewModels;

public class AddGameToListViewModel : IValidatableObject
{
    public int GameId { get; set; }

    [Required(ErrorMessage = "Game status is required.")]
    public string GameStatus { get; set; }

    [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
    public double? Rating { get; set; }

    [Range(1, 10, ErrorMessage = "Difficulty must be between 1 and 10.")]
    public double? Difficulty { get; set; }

    [MaxLength(500, ErrorMessage = "Review cannot exceed 500 characters.")]
    public string? Review { get; set; }

    public bool IsRecommended { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(Review) && !IsRecommendedSpecified)
        {
            yield return new ValidationResult(
                "You must specify whether you recommend the game when submitting a review.",
                new[] { nameof(IsRecommended) }
            );
        }
    }

    // Add this helper to detect whether checkbox was shown
    public bool IsRecommendedSpecified { get; set; } = false;
}
