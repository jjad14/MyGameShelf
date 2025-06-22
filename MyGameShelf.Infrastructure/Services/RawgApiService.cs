using Microsoft.Extensions.Options;
using MyGameShelf.Application.Configurations;
using MyGameShelf.Application.DTOs;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Infrastructure.External.Rawg.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace MyGameShelf.Infrastructure.Services;
public class RawgApiService : IRawgApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public RawgApiService(HttpClient httpClient, IOptions<RawgSettings> options)
    {
        _httpClient = httpClient;
        _apiKey = options.Value.ApiKey;
    }

    public async Task<IEnumerable<GameDto>> GetPopularGamesAsync(int page = 1, int pageSize = 20)
    {
        var url = $"https://api.rawg.io/api/games?key={_apiKey}&page={page}&page_size={pageSize}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result.Results.Select(r => new GameDto
        {
            Id = r.Id,
            Name = r.Name,
            Released = r.Released,
            BackgroundImage = r.BackgroundImage,
            Rating = r.Rating,
            Genres = r.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
            Tags = r.Tags?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
            
        });
    }

    public async Task<GameDto> GetGameDetailsAsync(int rawgId)
    {
        var url = $"https://api.rawg.io/api/games/{rawgId}?key={_apiKey}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGameDetailResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return new GameDto
        {
            Id = result.Id,
            Name = result.Name,
            Released = result.Released,
            BackgroundImage = result.BackgroundImage,
            Rating = result.Rating,
            Description = result.Description
        };
    }

    public async Task<IEnumerable<GameDto>> GetGamesBySearchAndFilters(string search, string platform, string developer, string publisher, string genre, string rating, string orderBy, int page = 1, int pageSize = 20)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["key"] = _apiKey;

        if (!string.IsNullOrWhiteSpace(search))
            query["search"] = search;

        if (!string.IsNullOrWhiteSpace(platform))
            query["platforms"] = platform; // RAWG expects platform IDs

        if (!string.IsNullOrWhiteSpace(developer))
            query["developers"] = developer;

        if (!string.IsNullOrWhiteSpace(publisher))
            query["publishers"] = publisher;

        if (!string.IsNullOrWhiteSpace(genre))
            query["genres"] = genre;

        if (!string.IsNullOrWhiteSpace(rating))
            query["metacritic"] = rating;

        if (!string.IsNullOrWhiteSpace(orderBy))
            query["ordering"] = orderBy;

        query["page"] = page.ToString();
        query["page_size"] = pageSize.ToString();

        string url = $"https://api.rawg.io/api/games?{query}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result.Results.Select(r => new GameDto
        {
            Id = r.Id,
            Name = r.Name,
            Released = r.Released,
            BackgroundImage = r.BackgroundImage,
            Rating = r.Rating
        });
    }


}
