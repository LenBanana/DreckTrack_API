using DreckTrack_API.Models.Entities.Base;

namespace DreckTrack_API.Models.Entities.Collectibles;

public class Episode : BaseEntity
{
    public Guid SeasonId { get; set; }
    public string Name { get; set; }
    public string ExternalId { get; set; }
    public bool Watched { get; set; }
    public int? EpisodeNumber { get; set; }
    public double? Duration { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Description { get; set; }
}