namespace DreckTrack_API.Models.Dto.Items.Show;

public class EpisodeDto
{
    public Guid? Id { get; set; }
    public Guid? SeasonId { get; set; }
    public string Name { get; set; }
    public string ExternalId { get; set; }
    public bool Watched { get; set; }
    public int? EpisodeNumber { get; set; }
    public double? Duration { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Description { get; set; }
}