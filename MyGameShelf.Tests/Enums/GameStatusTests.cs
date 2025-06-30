using MyGameShelf.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Tests.Enums;
public class GameStatusTests
{
    [Theory]
    [InlineData(GameStatus.Playing, "Playing")]
    [InlineData(GameStatus.Completed, "Completed")]
    [InlineData(GameStatus.OnHold, "On Hold")]
    [InlineData(GameStatus.Dropped, "Dropped")]
    [InlineData(GameStatus.Planned, "Planned")]
    [InlineData(GameStatus.Wishlist, "Wishlist")]
    public void GameStatus_ToLabel_ReturnsExpectedLabel(GameStatus status, string expected)
    {
        Assert.Equal(expected, status.ToLabel());
    }

    [Fact]
    public void GameStatus_DefaultValue_IsPlaying()
    {
        var status = default(GameStatus);

        Assert.Equal(GameStatus.Playing, status);
    }
}
