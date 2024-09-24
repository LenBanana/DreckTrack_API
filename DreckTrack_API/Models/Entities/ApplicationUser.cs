using DreckTrack_API.Models.Entities.Collectibles;
using DreckTrack_API.Models.Entities.Collectibles.Items;
using Microsoft.AspNetCore.Identity;

namespace DreckTrack_API.Models.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    // Additional profile properties
    public string DisplayName { get; set; }
    public ICollection<UserCollectibleItem> UserCollectibleItems { get; set; }
}