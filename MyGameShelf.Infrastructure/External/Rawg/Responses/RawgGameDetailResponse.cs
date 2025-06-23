using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.External.Rawg.Responses;

public class RawgGameDetailResponse
{
    public int Id { get; set; }
    public string Slug { get; set; }
    public string Name { get; set; }

    [JsonPropertyName("name_original")]
    public string NameOriginal { get; set; }

    [JsonPropertyName("alternative_names")]
    public List<string>? AlternativeNames { get; set; }
    public string Description { get; set; }  // Raw HTML

    [JsonPropertyName("description_raw")]
    public string DescriptionRaw { get; set; } // No HTML
    public int? Metacritic { get; set; }




    [JsonPropertyName("metacritic_platforms")]
    public List<MetacriticPlatform> MetacriticPlatforms { get; set; }

    public DateTime? Released { get; set; }
    public bool Tba { get; set; }
    public DateTime? Updated { get; set; }

    [JsonPropertyName("background_image")]
    public string BackgroundImage { get; set; }

    [JsonPropertyName("background_image_additional")]
    public string BackgroundImageAdditional { get; set; }
    public string Website { get; set; }

    public double? Rating { get; set; }

    [JsonPropertyName("rating_top")]
    public int? RatingTop { get; set; }
    public int? Added { get; set; }
    public int? Playtime { get; set; }

    [JsonPropertyName("achievements_count")]
    public int? AchievementsCount { get; set; }

    [JsonPropertyName("reddit_url")]
    public string? RedditUrl { get; set; }

    [JsonPropertyName("metacritic_url")]
    public string? MetacriticUrl { get; set; }

    [JsonPropertyName("game_series_count")]
    public int? GameSeriesCount { get; set; }
    public List<RawgPlatformWrapper> Platforms { get; set; }
    public List<RawgDeveloperSummary> Developers { get; set; }
    public List<RawgPublisherSummary> Publishers { get; set; }

    public IEnumerable<RawgGenres> Genres { get; set; }
    public IEnumerable<RawgTags> Tags { get; set; }

    [JsonPropertyName("esrb_rating")]
    public EsrbRating EsrbRating { get; set; }


}

public class MetacriticPlatform
{
    public int Metascore { get; set; }
    public string Url { get; set; }

    public RawgPlatform Platform { get; set; }
}

public class RawgPublisherSummary
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }

    [JsonPropertyName("image_background")]
    public string? ImageBackground { get; set; }
}
