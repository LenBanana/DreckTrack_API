using DreckTrack_API.Models.Entities.Base;
using DreckTrack_API.Models.Entities.Collectibles.Information;
using DreckTrack_API.Models.Enums;

namespace DreckTrack_API.Models.Entities.Collectibles;

public class UserCollectibleItem : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }

    public Guid? CollectibleItemId { get; set; }
    public CollectibleItem CollectibleItem { get; set; }

    public CollectibleStatus Status { get; set; }
    public int? UserRating { get; set; } // e.g., 1-5 stars
    public string Notes { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public DateTime? DateStarted { get; set; }
    public DateTime? DateCompleted { get; set; }
    // For shows, tracking episodes watched
    public ICollection<UserShowEpisodeProgress> EpisodeProgress { get; set; }
}