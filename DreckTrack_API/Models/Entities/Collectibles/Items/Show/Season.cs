using DreckTrack_API.Models.Entities.Base;

namespace DreckTrack_API.Models.Entities.Collectibles.Items.Show;

public class Season : BaseEntity
{
    public Guid ShowId { get; set; }
    public string Name { get; set; }
    public string ExternalId { get; set; }
    public int? SeasonNumber { get; set; }
    public List<Episode> Episodes { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Description { get; set; }
}