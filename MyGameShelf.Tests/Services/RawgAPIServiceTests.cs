using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using MyGameShelf.Application.Configurations;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Tests.Services;
public class RawgAPIServiceTests
{
    private readonly Mock<IOptions<RawgSettings>> _mockOptions;
    private readonly Mock<ICacheService> _mockCache;
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly HttpClient _httpClient;
    private readonly RawgApiService _service;

    public RawgAPIServiceTests()
    {
        _mockOptions = new Mock<IOptions<RawgSettings>>();
        _mockOptions.Setup(o => o.Value).Returns(new RawgSettings
        {
            ApiKey = "fake-api-key"
        });

        _mockCache = new Mock<ICacheService>();

        _mockHttpHandler = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri("https://api.rawg.io/")
        };

        _service = new RawgApiService(_httpClient, _mockOptions.Object, _mockCache.Object);
    }

    [Fact]
    public async Task GetPopularGamesAsync_ReturnsExpectedGames()
    {
        // Arrange

        // Setup sample JSON response similar to what RAWG returns
        var jsonResponse = @"
    {
        ""results"": [
            {
                ""id"": 1,
                ""name"": ""Game One"",
                ""released"": ""2022-01-01"",
                ""background_image"": ""https://image1.jpg"",
                ""rating"": 4.5,
                ""genres"": [{""name"": ""Action""}],
                ""tags"": [{""name"": ""Multiplayer""}]
            },
            {
                ""id"": 2,
                ""name"": ""Game Two"",
                ""released"": ""2021-06-15"",
                ""background_image"": """",
                ""rating"": 3.8,
                ""genres"": [{""name"": ""RPG""}],
                ""tags"": []
            }
        ]
    }";

        // Setup HttpClient mock to return this JSON when GetAsync is called
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            });

        // Act
        var result = await _service.GetPopularGamesAsync(page: 1, pageSize: 20);

        // Assert

        // Verify result is not null and has 2 games as in mock data
        Assert.NotNull(result);
        var games = result.ToList();
        Assert.Equal(2, games.Count);

        // Validate first game data mapped correctly
        var gameOne = games[0];
        Assert.Equal(1, gameOne.Id);
        Assert.Equal("Game One", gameOne.Name);
        Assert.Equal(DateTime.Parse("2022-01-01"), gameOne.Released);
        Assert.Equal("https://image1.jpg", gameOne.BackgroundImage);
        Assert.Equal(4.5, gameOne.Rating);
        Assert.Contains("Action", gameOne.Genres);
        Assert.Contains("Multiplayer", gameOne.Tags);

        // Validate second game uses default image since background_image is empty
        var gameTwo = games[1];
        Assert.Equal(2, gameTwo.Id);
        Assert.Equal("Game Two", gameTwo.Name);
        Assert.Equal(DateTime.Parse("2021-06-15"), gameTwo.Released);
        Assert.Equal("/assets/img/game_portrait_default.jpg", gameTwo.BackgroundImage);
        Assert.Equal(3.8, gameTwo.Rating);
        Assert.Contains("RPG", gameTwo.Genres);
        Assert.Empty(gameTwo.Tags);

        // Also verify HttpClient was called with expected URL containing API key and page params
        _mockHttpHandler.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get &&
                req.RequestUri.ToString().Contains($"key={_mockOptions.Object.Value.ApiKey}") &&
                req.RequestUri.ToString().Contains("page=1") &&
                req.RequestUri.ToString().Contains("page_size=20")),
            ItExpr.IsAny<CancellationToken>());
    }



}
