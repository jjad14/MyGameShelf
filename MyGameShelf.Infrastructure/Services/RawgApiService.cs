using Microsoft.Extensions.Caching.Memory;
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
    private readonly IMemoryCache _cache;

    public RawgApiService(HttpClient httpClient, IOptions<RawgSettings> options, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _apiKey = options.Value.ApiKey;
        _cache = cache;
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

    public async Task<PaginatedGameDto> GetGamesBySearchAndFilters(string search, string platform, string developer, string publisher, string genre, string rating, string orderBy, int page = 1, int pageSize = 20)
    {
        bool isDefaultSearch =
            string.IsNullOrWhiteSpace(search) &&
            string.IsNullOrWhiteSpace(platform) &&
            string.IsNullOrWhiteSpace(developer) &&
            string.IsNullOrWhiteSpace(publisher) &&
            string.IsNullOrWhiteSpace(genre) &&
            string.IsNullOrWhiteSpace(rating) &&
            string.IsNullOrWhiteSpace(orderBy) &&
            page == 1 &&
            pageSize == 20;
        string defaultCacheKey = "rawg_default_search_page_1_size_20";

        if (isDefaultSearch && _cache.TryGetValue(defaultCacheKey, out PaginatedGameDto cachedData))
        {
            return cachedData;
        }


        string cacheKey = $"rawg:search={search}|platform={platform}|developer={developer}|publisher={publisher}|genre={genre}|rating={rating}|order={orderBy}|page={page}|size={pageSize}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out PaginatedGameDto cachedResult))
        {
            return cachedResult;
        }

        // Otherwise make the API call
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

        // Requesting URL: https://api.rawg.io/api/games?key=c96b98c100004437a1b4ab2ffac25a1e&page=1&page_size=20
        //Console.WriteLine("Requesting URL: " + url);

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var paginatedGameDto =  new PaginatedGameDto
        {
            Games = result.Results.Select(r => new GameDto
            {
                Id = r.Id,
                Name = r.Name,
                Released = r.Released,
                BackgroundImage = r.BackgroundImage,
                Rating = r.Rating,
                Genres = r.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
                Tags = r.Tags?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
            }),
            TotalCount = result.Count
        };

        // Store in cache
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // or absolute expiration if preferred

        if (isDefaultSearch)
        {
            _cache.Set(defaultCacheKey, paginatedGameDto, cacheOptions); // cache for 10 min (adjust as needed)
        }
        else
        {
            _cache.Set(cacheKey, paginatedGameDto, cacheOptions);
        }


        return paginatedGameDto;

    }

    // tied to the code snippet in program.cs
    public async Task WarmUpPopularGameCache()
    {
        // Example default search (already cached now)
        await GetGamesBySearchAndFilters(null, null, null, null, null, null, null, 1, 20);

        // Example popular genre
        await GetGamesBySearchAndFilters(null, null, null, null, "action", null, null, 1, 20);

        // Example popular platform
        await GetGamesBySearchAndFilters(null, "4", null, null, null, null, null, 1, 20);
    }


}
