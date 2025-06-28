using Azure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MyGameShelf.Application.Configurations;
using MyGameShelf.Application.DTOs;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Domain.Models;
using MyGameShelf.Infrastructure.External.Rawg.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        // Check pagesize to ensure no tampering
        // Ensure pageSize is always between 1 and 50
        pageSize = Math.Clamp(pageSize, 1, 50);

        var url = $"https://api.rawg.io/api/games?key={_apiKey}&page={page}&page_size={pageSize}";
        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result.Results.Select(r => new GameDto
        {
            Id = r.Id,
            Name = r.Name,
            Released = r.Released,
            BackgroundImage = string.IsNullOrEmpty(r.BackgroundImage)
                    ? "/assets/img/game_portrait_default.jpg"  // your default relative path in wwwroot
                    : r.BackgroundImage,
            Rating = r.Rating,
            Genres = r.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
            Tags = r.Tags?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
            
        });
    }

    public async Task<GameDetailDto> GetGameDetailsAsync(int rawgId)
    {
        var url = $"https://api.rawg.io/api/games/{rawgId}?key={_apiKey}";
        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGameDetailResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return new GameDetailDto
        {
            Id = result.Id,
            Name = result.Name,
            Description = result.DescriptionRaw,
            Metacritic = result.Metacritic,
            Released = result.Released,
            BackgroundImage = string.IsNullOrEmpty(result.BackgroundImage)
                    ? "/assets/img/game_portrait_default.jpg"  // your default relative path in wwwroot
                    : result.BackgroundImage,
            EsrbRating = result.EsrbRating.Name ?? "Not Rated",
            Genres = result.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
            Tags = result.Tags?.Where(t => t.Language == "eng").Select(g => g.Name) ?? Enumerable.Empty<string>(),
            Platforms = result.Platforms?.Select(p => p.Platform.Name) ?? Enumerable.Empty<string>(),
            Developers = result.Developers?.Select(p => p.Name) ?? Enumerable.Empty<string>(),
            Publishers = result.Publishers?
                .Where(p => p != null)
                .Select(p => new GameDetailsPublisherDto
                { 
                    Id = p.Id,
                    Name = p.Name,
                    Slug = p.Slug
                }) ?? Enumerable.Empty<GameDetailsPublisherDto>(),
            PlatformRequirements = result.Platforms?
                .Where(p => p.Requirements != null)
                .Select(p => new PlatformRequirementsDto
                {
                    Name = p.Platform.Name,
                    MinimumRequirements = p.Requirements?.Minimum,
                    RecommendedRequirements = p.Requirements?.Recommended
                }) ?? Enumerable.Empty<PlatformRequirementsDto>(),
        };
    }

    public async Task<PaginatedGameDto> GetGamesBySearchAndFilters(string search, string platform, string developer, string publisher, string genre, string metacritic, string orderBy, int page = 1, int pageSize = 20)
    {
        // Check pagesize to ensure no tampering
        // Ensure pageSize is always between 1 and 50
        pageSize = Math.Clamp(pageSize, 1, 50);

        bool isDefaultSearch =
            string.IsNullOrWhiteSpace(search) &&
            string.IsNullOrWhiteSpace(platform) &&
            string.IsNullOrWhiteSpace(developer) &&
            string.IsNullOrWhiteSpace(publisher) &&
            string.IsNullOrWhiteSpace(genre) &&
            string.IsNullOrWhiteSpace(metacritic) &&
            string.IsNullOrWhiteSpace(orderBy) &&
            page == 1 &&
            pageSize == 20;
        string defaultCacheKey = "rawg_default_search_page_1_size_20";

        if (isDefaultSearch && _cache.TryGetValue(defaultCacheKey, out PaginatedGameDto cachedData))
        {
            return cachedData;
        }


        string cacheKey = $"rawg:search={search}|platform={platform}|developer={developer}|publisher={publisher}|genre={genre}|metacritic={metacritic}|order={orderBy}|page={page}|size={pageSize}";

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

        if (!string.IsNullOrWhiteSpace(metacritic))
            query["metacritic"] = metacritic;

        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            query["ordering"] = orderBy;
        }
        //else
        //{
        //    query["ordering"] = "-released";
        //}

        query["page"] = page.ToString();
        query["page_size"] = pageSize.ToString();

        string url = $"https://api.rawg.io/api/games?{query}&search_precise=true";

        using var response = await _httpClient.GetAsync(url);
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
                BackgroundImage = string.IsNullOrEmpty(r.BackgroundImage)
                    ? "/assets/img/game_portrait_default.jpg"  // your default relative path in wwwroot
                    : r.BackgroundImage,
                Metacritic = r.Metacritic,
                Genres = r.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
                Tags = r.Tags?.Where(t => t.Language == "eng").Select(g => g.Name) ?? Enumerable.Empty<string>(),
                Platforms = r.Platforms?.Select(p => p.Platform.Name) ?? Enumerable.Empty<string>(),
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

    public async Task<IEnumerable<GenreDto>> GetGenresAsync(int page = 1, int pageSize = 20)
    {
        // Check pagesize to ensure no tampering
        // Ensure pageSize is always between 1 and 50
        pageSize = Math.Clamp(pageSize, 1, 50);

        var url = $"https://api.rawg.io/api/genres?key={_apiKey}&page={page}&page_size={pageSize}";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGenreListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result.Results.Select(r => new GenreDto
        {
            Id = r.Id,
            Name = r.Name,
            GamesCount = r.GamesCount,
            ImageBackground = r.ImageBackground
        });
    }

    public async Task<IEnumerable<PlatformDto>> GetPlatformsAsync(int page = 1, int pageSize = 20)
    {
        // Check pagesize to ensure no tampering
        // Ensure pageSize is always between 1 and 50
        pageSize = Math.Clamp(pageSize, 1, 50);

        var url = $"https://api.rawg.io/api/platforms?key={_apiKey}&page={page}&page_size={pageSize}";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgPlatformListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result.Results.Select(r => new PlatformDto
        {
            Id = r.Id,
            Name = r.Name,
            GamesCount = r.GamesCount,
            ImageBackground = r.ImageBackground
        });
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPublisher(string publishers, int? excludeId = null)
    {
        // publisher = id or slug
        string url = $"https://api.rawg.io/api/games?key={_apiKey}&publishers={publishers}&ordering=-released&search_precise=true";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result?.Results == null)
        { 
            return Enumerable.Empty<GameDto>();
        }


        var gameDtos = result.Results
            .Where(r => !excludeId.HasValue || r.Id != excludeId.Value) // exclude current game if specified
            .Select(r => new GameDto
            {
                Id = r.Id,
                Name = r.Name,
                Released = r.Released,
                BackgroundImage = string.IsNullOrEmpty(r.BackgroundImage)
                    ? "/assets/img/game_portrait_default.jpg"  // your default relative path in wwwroot
                    : r.BackgroundImage,
                Metacritic = r.Metacritic,
                Genres = r.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
                Tags = r.Tags?.Where(t => t.Language == "eng").Select(g => g.Name) ?? Enumerable.Empty<string>(),
                Platforms = r.Platforms?.Select(p => p.Platform.Name) ?? Enumerable.Empty<string>(),
            }) ?? Enumerable.Empty<GameDto>();

        return gameDtos;
    }

    public async Task<bool> HasOtherGamesByPublisher(string publisherIds, int currentGameId)
    {
        string url = $"https://api.rawg.io/api/games?key={_apiKey}&publishers={publisherIds}&page_size=2&ordering=-released&search_precise=true";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result?.Results?.Any(g => g.Id != currentGameId) == true;
    }

    //Get Games dlcs
    public async Task<IEnumerable<GameDto>> GetGamesDLCs(int? gameId)
    {
        string url = $"https://api.rawg.io/api/games/{gameId}/additions?key={_apiKey}&ordering=-released";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result?.Results == null)
        {
            return Enumerable.Empty<GameDto>();
        }

        var gameDtos = result.Results
            .Where(r => !gameId.HasValue || r.Id != gameId.Value) // exclude current game if specified
            .Select(r => new GameDto
            {
                Id = r.Id,
                Name = r.Name,
                Released = r.Released,
                BackgroundImage = string.IsNullOrEmpty(r.BackgroundImage)
                        ? "/assets/img/game_portrait_default.jpg"  // your default relative path in wwwroot
                        : r.BackgroundImage,
                Metacritic = r.Metacritic,
                Genres = r.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
                Tags = r.Tags?.Where(t => t.Language == "eng").Select(g => g.Name) ?? Enumerable.Empty<string>(),
                Platforms = r.Platforms?.Select(p => p.Platform.Name) ?? Enumerable.Empty<string>(),
            });

        return gameDtos;
    }

    public async Task<bool> HasGameDLCs(int gameId)
    {
        string url = $"https://api.rawg.io/api/games/{gameId}/additions?key={_apiKey}&page_size=2&search_precise=true";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result?.Results?.Any(g => g.Id != gameId) == true;
    }

    //Get Games sequels
    public async Task<IEnumerable<GameDto>> GetGamesSequels(int? gameId)
    {
        string url = $"https://api.rawg.io/api/games/{gameId}/game-series?key={_apiKey}&ordering=-released";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result?.Results == null)
        {
            return Enumerable.Empty<GameDto>();
        }

        var gameDtos = result.Results
            .Where(r => !gameId.HasValue || r.Id != gameId.Value) // exclude current game if specified
            .Select(r => new GameDto
            {
                Id = r.Id,
                Name = r.Name,
                Released = r.Released,
                BackgroundImage = string.IsNullOrEmpty(r.BackgroundImage)
                        ? "/assets/img/game_portrait_default.jpg"  // your default relative path in wwwroot
                        : r.BackgroundImage,
                Metacritic = r.Metacritic,
                Genres = r.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
                Tags = r.Tags?.Where(t => t.Language == "eng").Select(g => g.Name) ?? Enumerable.Empty<string>(),
                Platforms = r.Platforms?.Select(p => p.Platform.Name) ?? Enumerable.Empty<string>(),
            });

        return gameDtos;
    }

    public async Task<bool> HasGameSequels(int gameId)
    {
        string url = $"https://api.rawg.io/api/games/{gameId}/game-series?key={_apiKey}&page_size=2&search_precise=true";

        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result?.Results?.Any(g => g.Id != gameId) == true;
    }

    // Cache common rawg api calls
    public async Task WarmUpPopularGameCache()
    {
        // Example default search (already cached now)
        await GetGamesBySearchAndFilters(null, null, null, null, null, null, null, 1, 20);

        // Example popular platform
        await GetGamesBySearchAndFilters(null, "4", null, null, null, null, null, 1, 20);
        await GetGamesBySearchAndFilters(null, "187", null, null, null, null, null, 1, 20);
        await GetGamesBySearchAndFilters(null, "1", null, null, null, null, null, 1, 20);
        await GetGamesBySearchAndFilters(null, "18", null, null, null, null, null, 1, 20);
        await GetGamesBySearchAndFilters(null, "186", null, null, null, null, null, 1, 20);
        await GetGamesBySearchAndFilters(null, "7", null, null, null, null, null, 1, 20);
    }

}
