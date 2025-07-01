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
        var review = new Review("user1", 1, "Initial content", false);
        var newContent = "Updated content";
        var newRating = true;

        // Act
        review.UpdateReview(newContent, newRating);

        // Assert
        Assert.Equal(newContent, review.Content);
        Assert.Equal(newRating, review.IsRecommended);
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
        var review = new Review("user1", 1, "Initial content", true);

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => review.UpdateReview(invalidContent!, false));
        Assert.Contains("Review content cannot be empty", ex.Message);
    }
}