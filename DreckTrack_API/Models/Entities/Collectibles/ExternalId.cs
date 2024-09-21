using DreckTrack_API.Models.Entities.Base;

namespace DreckTrack_API.Models.Entities.Collectibles;

public class ExternalId : BaseEntity
{
    public string Source { get; set; } // e.g., "ISBN", "IMDB", "TMDB"
    public string Identifier { get; set; }
    public Guid CollectibleItemId { get; set; }
    public CollectibleItem CollectibleItem { get; set; }
}
