using DreckTrack_API.Models.Entities.Base;

namespace DreckTrack_API.Models.Entities.Collectibles.Information;

public class UserShowEpisodeProgress : BaseEntity
{
    public Guid UserCollectibleItemId { get; set; }
    public UserCollectibleItem UserCollectibleItem { get; set; }

    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public bool Watched { get; set; }
    public DateTime? WatchedOn { get; set; }
}
