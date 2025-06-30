using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Tests.Entities;
public class ReviewTests
{
    [Fact]
    public void UpdateReview_ValidInput_UpdatesPropertiesAndSetsUpdatedAt()
    {
        // Arrange
        var review = new Review("user1", 1, "Initial content", 7.5);
        var newContent = "Updated content";
        var newRating = 9.0;

        // Act
        review.UpdateReview(newContent, newRating);

        // Assert
        Assert.Equal(newContent, review.Content);
        Assert.Equal(newRating, review.Rating);
        Assert.NotNull(review.UpdatedAt);
        Assert.True(review.UpdatedAt > review.CreatedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateReview_InvalidContent_ThrowsArgumentException(string invalidContent)
    {
        // Arrange
        var review = new Review("user1", 1, "Initial content", 7.5);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => review.UpdateReview(invalidContent!, 5.0));
        Assert.Contains("Review content cannot be empty", ex.Message);
    }

    [Theory]
    [InlineData(0.9)]
    [InlineData(10.1)]
    [InlineData(-1)]
    public void UpdateReview_InvalidRating_ThrowsArgumentOutOfRangeException(double invalidRating)
    {
        // Arrange
        var review = new Review("user1", 1, "Initial content", 7.5);

        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => review.UpdateReview("Valid content", invalidRating));
        Assert.Contains("Rating must be between 1.0 and 10.0", ex.Message);
    }
}