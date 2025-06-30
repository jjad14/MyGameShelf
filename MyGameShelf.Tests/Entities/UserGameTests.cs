using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Tests.Entities;
public class UserGameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(1.0)]
    [InlineData(5.5)]
    [InlineData(10.0)]
    public void SetDifficulty_ValidValues_SetsDifficulty(double? validValue)
    {
        // Arrange
        var userGame = new UserGame("user1", 1);

        // Act
        userGame.SetDifficulty(validValue);

        // Assert
        Assert.Equal(validValue, userGame.Difficulty);
    }

    [Theory]
    [InlineData(0.9)]
    [InlineData(10.1)]
    [InlineData(-5)]
    public void SetDifficulty_InvalidValues_ThrowsArgumentOutOfRangeException(double invalidValue)
    {
        // Arrange
        var userGame = new UserGame("user1", 1);

        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => userGame.SetDifficulty(invalidValue));
        Assert.Contains("Difficulty must be between 1.0 and 10.0 inclusive.", ex.Message);
    }
}
