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

    Task<GameDetailDto> GetGameDetailsAsync(int rawgId);

    Task<PaginatedGameDto> GetGamesBySearchAndFilters(string search, string platform, string developer, string publisher, string genre, string rating, string orderBy, int page = 1, int pageSize = 20);

    // Get Platforms
    Task<IEnumerable<PlatformDto>> GetPlatformsAsync(int page = 1, int pageSize = 20);

    // Get Genres
    Task<IEnumerable<GenreDto>> GetGenresAsync(int page = 1, int pageSize = 20);

    //Get Games by publisher - sort by newest
    Task<IEnumerable<GameDto>> GetGamesByPublisher(string publisher, int? excludeId = null);

    Task<bool> HasOtherGamesByPublisher(string publisherIds, int currentGameId);

    //Get Games dlcs
    Task<IEnumerable<GameDto>> GetGamesDLCs(string gameId);

    //Get Games sequels
    Task<IEnumerable<GameDto>> GetGamesSequels(string gameId);


    // For caching popular/default searches
    Task WarmUpPopularGameCache();
}
