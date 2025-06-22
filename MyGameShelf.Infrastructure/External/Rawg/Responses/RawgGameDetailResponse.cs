using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.External.Rawg.Responses;

public class RawgGameDetailResponse
{
    public int Id { get; set; }
    public string Slug { get; set; }
    public string Name { get; set; }
    public string NameOriginal { get; set; }
    public string Description { get; set; }  // Raw HTML
    public int? Metacritic { get; set; }
    public List<MetacriticPlatform> MetacriticPlatforms { get; set; }

    public DateTime? Released { get; set; }
    public bool Tba { get; set; }
    public DateTime? Updated { get; set; }

    public string BackgroundImage { get; set; }
    public string BackgroundImageAdditional { get; set; }
    public string Website { get; set; }

    public double? Rating { get; set; }
    public int? RatingTop { get; set; }
    public int? RatingsCount { get; set; }
    public int? SuggestionsCount { get; set; }
    public int? Added { get; set; }
    public int? Playtime { get; set; }

    public int? ScreenshotsCount { get; set; }
    public int? MoviesCount { get; set; }
    public int? CreatorsCount { get; set; }
    public int? AchievementsCount { get; set; }
    public int? ParentsCount { get; set; }
    public int? AdditionsCount { get; set; }
    public int? GameSeriesCount { get; set; }

    public string RedditUrl { get; set; }
    public string RedditName { get; set; }
    public string RedditDescription { get; set; }
    public string RedditLogo { get; set; }
    public int? RedditCount { get; set; }

    public string TwitchCount { get; set; }
    public string YoutubeCount { get; set; }
    public string ReviewsTextCount { get; set; }

    public List<string> AlternativeNames { get; set; }
    public string MetacriticUrl { get; set; }

    public EsrbRating EsrbRating { get; set; }
    public List<RawgPlatformWrapper> Platforms { get; set; }
}

public class MetacriticPlatform
{
    public int Metascore { get; set; }
    public string Url { get; set; }
}
