using MyGameShelf.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyGameShelf.Application.External.Rawg.Responses;

// Reponses from RAWG API
public class RawgGameDetailResponse
{
    public int Id { get; set; }

    public string Name { get; set; }
    
    [JsonPropertyName("slug")]
    public string? Slug { get; set; }
    public string SlugOrDefault => !string.IsNullOrWhiteSpace(Slug) ? Slug : StringHelpers.ToKebabCase(Name);

    [JsonPropertyName("name_original")]
    public string NameOriginal { get; set; }
    public string NameOriginalOrDefault => NameOriginal ?? Name;

    [JsonPropertyName("alternative_names")]
    public List<string>? AlternativeNames { get; set; } = new();
    public string Description { get; set; }  // Raw HTML

    [JsonPropertyName("description_raw")]
    public string DescriptionRaw { get; set; } // No HTML
    public string DescriptionOrDefault => !string.IsNullOrWhiteSpace(DescriptionRaw) ? DescriptionRaw : Description;

    public int? Metacritic { get; set; }

    [JsonPropertyName("metacritic_platforms")]
    public List<MetacriticPlatform> MetacriticPlatforms { get; set; }

    public DateTime? Released { get; set; }
    public bool Tba { get; set; }
    public DateTime? Updated { get; set; }

    [JsonPropertyName("background_image")]
    public string BackgroundImage { get; set; }
    public string BackgroundImageOrDefault => BackgroundImage ?? "/assets/img/game_wide_default.jpg";

    [JsonPropertyName("background_image_additional")]
    public string BackgroundImageAdditional { get; set; }
    public string Website { get; set; }
    public string WebsiteOrDefault => string.IsNullOrWhiteSpace(Website) ? "Not available" : Website;


    public double? Rating { get; set; }

    [JsonPropertyName("rating_top")]
    public int? RatingTop { get; set; }
    public int? Added { get; set; }
    public int? Playtime { get; set; }

    [JsonPropertyName("achievements_count")]
    public int? AchievementsCount { get; set; }

    [JsonPropertyName("reddit_url")]
    public string? RedditUrl { get; set; }
    public string RedditUrlOrDefault => RedditUrl ?? "#";


    [JsonPropertyName("metacritic_url")]
    public string? MetacriticUrl { get; set; }
    public string MetacriticUrlOrDefault => MetacriticUrl ?? "#";


    [JsonPropertyName("game_series_count")]
    public int? GameSeriesCount { get; set; }
    public List<RawgPlatformWrapper> Platforms { get; set; } = new();
    public List<RawgDeveloperSummary> Developers { get; set; } = new();
    public List<RawgPublisherSummary> Publishers { get; set; } = new();

    public IEnumerable<RawgGenres> Genres { get; set; } = Enumerable.Empty<RawgGenres>();
    public IEnumerable<RawgTags> Tags { get; set; } = Enumerable.Empty<RawgTags>();

    [JsonPropertyName("esrb_rating")]
    public EsrbRating? EsrbRating { get; set; }
    public string EsrbRatingName => EsrbRating?.Name ?? "Not Rated";
    public string EsrbRatingSlug => EsrbRating?.Slug ?? "not-rated";

}

public class MetacriticPlatform
{
    public int Metascore { get; set; }
    public string Url { get; set; } = "#";

    public RawgPlatform Platform { get; set; } = new();
}

public class RawgPublisherSummary
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Slug { get; set; }
    public string SlugOrDefault => !string.IsNullOrWhiteSpace(Slug) ? Slug : StringHelpers.ToKebabCase(Name);

    [JsonPropertyName("image_background")]
    public string? ImageBackground { get; set; }
}
