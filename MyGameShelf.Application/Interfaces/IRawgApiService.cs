using MyGameShelf.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Application.Interfaces;
public interface IRawgApiService
{
    Task<IEnumerable<GameDto>> GetPopularGamesAsync(int page = 1, int pageSize = 20);

    Task<GameDto> GetGameDetailsAsync(int rawgId);

    Task<PaginatedGameDto> GetGamesBySearchAndFilters(string search, string platform, string developer, string publisher, string genre, string rating, string orderBy, int page = 1, int pageSize = 20);

    // For caching popular/default searches
    Task WarmUpPopularGameCache();

    // Get Developers
    Task<IEnumerable<DeveloperDto>> GetDevelopersAsync(int page = 1, int pageSize = 20);

    // Get Platforms
    Task<IEnumerable<PlatformDto>> GetPlatformsAsync(int page = 1, int pageSize = 20);

    // Get Genres
    Task<IEnumerable<GenreDto>> GetGenresAsync(int page = 1, int pageSize = 20);

    // Get Publishers
    Task GetPublishersAsync();

    // Get Creators
    Task GetCreatorsAsync();
}
