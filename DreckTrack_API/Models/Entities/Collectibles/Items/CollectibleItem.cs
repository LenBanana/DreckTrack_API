using DreckTrack_API.Models.Entities.Base;
using DreckTrack_API.Models.Entities.Collectibles.Information;

namespace DreckTrack_API.Models.Entities.Collectibles.Items;

public class CollectibleItem : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? Language { get; set; }
    public List<string>? Genres { get; set; }
    public List<string>? Tags { get; set; }
    public string? CoverImageUrl { get; set; }
    public double? AverageRating { get; set; }
    public int? RatingsCount { get; set; }
    public List<ExternalId>? ExternalIds { get; set; }
    public string ItemType { get; set; } // "Book", "Movie", "Show"
}