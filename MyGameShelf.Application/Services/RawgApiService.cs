using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MyGameShelf.Application.Configurations;
using MyGameShelf.Application.DTOs;
using MyGameShelf.Application.Exceptions;
using MyGameShelf.Application.External.Rawg.Responses;
using MyGameShelf.Application.Interfaces;
using MyGameShelf.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace MyGameShelf.Application.Services;
public class RawgApiService : IRawgApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ICacheService _cache;

    public RawgApiService(HttpClient httpClient, IOptions<RawgSettings> options, ICacheService cache)
    {
        _httpClient = httpClient;
        _apiKey = options.Value.ApiKey;
        _cache = cache;
    }

    public async Task<IEnumerable<GameDto>> GetPopularGamesAsync(int page = 1, int pageSize = 20)
    {
        try
        {
            // Check pagesize to ensure no tampering
            pageSize = Math.Clamp(pageSize, 1, 50);

            string cacheKey = $"rawg:popular_games:page={page}|size={pageSize}";

            var cached = await _cache.GetAsync<IEnumerable<GameDto>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

            var url = $"https://api.rawg.io/api/games?key={_apiKey}&page={page}&page_size={pageSize}";
            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var gameList = result.Results.Select(r => new GameDto
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


            // Cache the result for 30 minutes
            await _cache.SetAsync(cacheKey, gameList, TimeSpan.FromMinutes(30));

            return gameList;
        }
        catch (Exception ex)
        {
            // Option 1: 
            // return Enumerable.Empty<GameDto>();

            // Option 2 (better): throw a custom exception
            throw new RawgApiException("Failed to fetch popular games from RAWG.", ex);
        }
    }

    public async Task<GameDetailDto> GetGameDetailsAsync(int rawgId)
    {
        try
        {
            string cacheKey = $"rawg:game_details:{rawgId}";

            // Try to retrieve from cache
            var cached = await _cache.GetAsync<GameDetailDto>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

            // Fetch from RAWG
            var url = $"https://api.rawg.io/api/games/{rawgId}?key={_apiKey}";
            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RawgGameDetailResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var gameDto =  new GameDetailDto
            {
                Id = result.Id,
                Name = result.Name,
                Slug = result.Slug,
                Description = result.DescriptionRaw,
                Metacritic = result.Metacritic,
                Released = result.Released,
                Rating = result.Rating,
                BackgroundImage = result.BackgroundImageOrDefault,
                EsrbRating = result.EsrbRatingName,
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

            // Cache the result for 1 hour
            await _cache.SetAsync(cacheKey, gameDto, TimeSpan.FromHours(1));

            return gameDto;
        }
        catch (Exception ex)
        {
            throw new RawgApiException("Failed to fetch game details from RAWG.", ex);
        }
    }

    public async Task<PaginatedGameDto> GetGamesBySearchAndFilters(string search, string platform, string developer, string publisher, string genre, string metacritic, string orderBy, int page = 1, int pageSize = 20)
    {
        try
        {
            // Check pagesize to ensure no tampering
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

            string cacheKey = isDefaultSearch
                        ? defaultCacheKey
                        : $"rawg:search={search}|platform={platform}|developer={developer}|publisher={publisher}|genre={genre}|metacritic={metacritic}|order={orderBy}|page={page}|size={pageSize}";

            var cachedResult = await _cache.GetAsync<PaginatedGameDto>(cacheKey);

            // Try to get from cache
            if (cachedResult != null)
            {
                return cachedResult;
            }

            // Build API query
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["key"] = _apiKey;

            if (!string.IsNullOrWhiteSpace(search)) query["search"] = search;

            if (!string.IsNullOrWhiteSpace(platform)) query["platforms"] = platform; // RAWG expects platform IDs

            if (!string.IsNullOrWhiteSpace(developer)) query["developers"] = developer;

            if (!string.IsNullOrWhiteSpace(publisher)) query["publishers"] = publisher;

            if (!string.IsNullOrWhiteSpace(genre)) query["genres"] = genre;

            if (!string.IsNullOrWhiteSpace(metacritic)) query["metacritic"] = metacritic;

            if (!string.IsNullOrWhiteSpace(orderBy)) query["ordering"] = orderBy;

            query["page"] = page.ToString();
            query["page_size"] = pageSize.ToString();

            string url = $"https://api.rawg.io/api/games?{query}&search_precise=true";

            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var paginatedGameDto = new PaginatedGameDto
            {
                Games = result.Results.Select(r => new GameDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Released = r.Released,
                    BackgroundImage = r.BackgroundImageOrDefault,
                    Metacritic = r.Metacritic,
                    Genres = r.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
                    Tags = r.Tags?.Where(t => t.Language == "eng").Select(g => g.Name) ?? Enumerable.Empty<string>(),
                    Platforms = r.Platforms?.Select(p => p.Platform.Name) ?? Enumerable.Empty<string>(),
                    Search = search,
                    Platform = platform,
                    Genre = genre,
                    MetacriticFilter = metacritic,
                    OrderBy = orderBy,
                    Page = page,
                    PageSize = pageSize
                }),
                TotalCount = result.Count
            };

            // Cache the result for 10 minutes
            await _cache.SetAsync(cacheKey, paginatedGameDto, TimeSpan.FromMinutes(10));

            return paginatedGameDto;
        }
        catch (Exception ex)
        {
            throw new RawgApiException("Failed to fetch games from RAWG.", ex);
        }

    }

    public async Task<IEnumerable<GenreDto>> GetGenresAsync(int page = 1, int pageSize = 20)
    {
        try
        {
            // Check pagesize to ensure no tampering
            pageSize = Math.Clamp(pageSize, 1, 50);

            string cacheKey = $"rawg:genres:page={page}|size={pageSize}";

            var cached = await _cache.GetAsync<IEnumerable<GenreDto>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

            var url = $"https://api.rawg.io/api/genres?key={_apiKey}&page={page}&page_size={pageSize}";

            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RawgGenreListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var genreList = result.Results.Select(r => new GenreDto
            {
                Id = r.Id,
                Name = r.Name,
                GamesCount = r.GamesCount,
                ImageBackground = r.ImageBackgroundOrDefault
            });

            // Cache for 12 hours since genres rarely change
            await _cache.SetAsync(cacheKey, genreList, TimeSpan.FromHours(12));

            return genreList;
        }
        catch (Exception ex)
        {
            throw new RawgApiException("Failed to fetch genres from RAWG.", ex);
        }

    }

    public async Task<IEnumerable<PlatformDto>> GetPlatformsAsync(int page = 1, int pageSize = 20)
    {
        try
        {
            // Check pagesize to ensure no tampering
            pageSize = Math.Clamp(pageSize, 1, 50);

            string cacheKey = $"rawg:platforms:page={page}|size={pageSize}";

            var cached = await _cache.GetAsync<IEnumerable<PlatformDto>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

            var url = $"https://api.rawg.io/api/platforms?key={_apiKey}&page={page}&page_size={pageSize}";

            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RawgPlatformListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var platformList = result.Results.Select(r => new PlatformDto
            {
                Id = r.Id,
                Name = r.Name,
                GamesCount = r.GamesCount,
                ImageBackground = r.ImageBackgroundOrDefault
            });

            // Cache for 2 weeks since platforms rarely change
            await _cache.SetAsync(cacheKey, platformList, TimeSpan.FromDays(14));

            return platformList;
        }
        catch (Exception ex)
        {
            throw new RawgApiException("Failed to fetch platforms from RAWG.", ex);
        }

    }

    public async Task<IEnumerable<GameDto>> GetGamesByPublisher(string publishers, int? excludeId = null)
    {
        try
        {
            string cacheKey = $"rawg:publisher:publishers={publishers}|excludeId={excludeId}";

            var cached = await _cache.GetAsync<IEnumerable<GameDto>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

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
                    BackgroundImage = r.BackgroundImageOrDefault,
                    Metacritic = r.Metacritic,
                    Genres = r.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
                    Tags = r.Tags?.Where(t => t.Language == "eng").Select(g => g.Name) ?? Enumerable.Empty<string>(),
                    Platforms = r.Platforms?.Select(p => p.Platform.Name) ?? Enumerable.Empty<string>(),
                }) ?? Enumerable.Empty<GameDto>();


            // Cache for 2 weeks since publishers rarely change
            await _cache.SetAsync(cacheKey, gameDtos, TimeSpan.FromDays(14));

            return gameDtos;
        }
        catch (Exception ex)
        {
            throw new RawgApiException("Failed to fetch games by publisher from RAWG.", ex);
        }

    }

    public async Task<bool> HasOtherGamesByPublisher(string publisherIds, int currentGameId)
    {
        try
        {
            string url = $"https://api.rawg.io/api/games?key={_apiKey}&publishers={publisherIds}&page_size=2&ordering=-released&search_precise=true";

            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Results?.Any(g => g.Id != currentGameId) == true;
        }
        catch (Exception ex)
        {
            //throw new RawgApiException("Failed to check related games by publisher from RAWG.", ex);
            return false;
        }

    }

    //Get Games dlcs
    public async Task<IEnumerable<GameDto>> GetGamesDLCs(int? gameId)
    {
        try
        {
            string cacheKey = $"rawg:additions:gameId={gameId}";

            var cached = await _cache.GetAsync<IEnumerable<GameDto>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

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
                    BackgroundImage = r.BackgroundImageOrDefault,
                    Metacritic = r.Metacritic,
                    Genres = r.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
                    Tags = r.Tags?.Where(t => t.Language == "eng").Select(g => g.Name) ?? Enumerable.Empty<string>(),
                    Platforms = r.Platforms?.Select(p => p.Platform.Name) ?? Enumerable.Empty<string>(),
                });

            // Cache for 2 weeks since platforms rarely change
            await _cache.SetAsync(cacheKey, gameDtos, TimeSpan.FromDays(14));

            return gameDtos;
        }
        catch (Exception ex)
        {
            throw new RawgApiException("Failed to fetch games additions from RAWG.", ex);
        }

    }

    public async Task<bool> HasGameDLCs(int gameId)
    {
        try
        {
            string url = $"https://api.rawg.io/api/games/{gameId}/additions?key={_apiKey}&page_size=2&search_precise=true";

            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Results?.Any(g => g.Id != gameId) == true;
        }
        catch (Exception ex)
        {
            //throw new RawgApiException("Failed to check for game additions from RAWG.", ex);
            return false;
        }

    }

    //Get Games sequels
    public async Task<IEnumerable<GameDto>> GetGamesSequels(int? gameId)
    {
        try
        {
            string cacheKey = $"rawg:sequels:gameId={gameId}";

            var cached = await _cache.GetAsync<IEnumerable<GameDto>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

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
                    BackgroundImage = r.BackgroundImageOrDefault,
                    Metacritic = r.Metacritic,
                    Genres = r.Genres?.Select(g => g.Name) ?? Enumerable.Empty<string>(),
                    Tags = r.Tags?.Where(t => t.Language == "eng").Select(g => g.Name) ?? Enumerable.Empty<string>(),
                    Platforms = r.Platforms?.Select(p => p.Platform.Name) ?? Enumerable.Empty<string>(),
                });

            // Cache for 2 weeks since platforms rarely change
            await _cache.SetAsync(cacheKey, gameDtos, TimeSpan.FromDays(14));

            return gameDtos;
        }
        catch (Exception ex)
        {
            throw new RawgApiException("Failed to fetch game additions from RAWG.", ex);
        }

    }

    public async Task<bool> HasGameSequels(int gameId)
    {
        try
        {
            string url = $"https://api.rawg.io/api/games/{gameId}/game-series?key={_apiKey}&page_size=2&search_precise=true";

            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RawgGameListResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result?.Results?.Any(g => g.Id != gameId) == true;
        }
        catch (Exception ex)
        {

            //throw new RawgApiException("Failed to check for game additions from RAWG.", ex);
            return false;
        }

    }

    // Cache common rawg api calls
    public async Task WarmUpPopularGameCache()
    {
        var cacheWarmupCalls = new List<Func<Task>>
    {
        () => GetGamesBySearchAndFilters(null, null, null, null, null, null, null, 1, 20),
        () => GetGamesBySearchAndFilters(null, "4", null, null, null, null, null, 1, 20),
        () => GetGamesBySearchAndFilters(null, "187", null, null, null, null, null, 1, 20),
        () => GetGamesBySearchAndFilters(null, "1", null, null, null, null, null, 1, 20),
        () => GetGamesBySearchAndFilters(null, "18", null, null, null, null, null, 1, 20),
        () => GetGamesBySearchAndFilters(null, "186", null, null, null, null, null, 1, 20),
        () => GetGamesBySearchAndFilters(null, "7", null, null, null, null, null, 1, 20),
    };

        foreach (var call in cacheWarmupCalls)
        {
            try
            {
                await call();
            }
            catch (Exception ex)
            {
                // Change to logging
                Console.WriteLine($"Cache warm-up call failed: {ex.Message}");
            }
        }
    }


}
