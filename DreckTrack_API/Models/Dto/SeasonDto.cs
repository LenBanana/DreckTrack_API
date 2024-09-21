namespace DreckTrack_API.Models.Dto;

public class SeasonDto
{
    public Guid? Id { get; set; }
    public Guid? ShowId { get; set; }
    public string Name { get; set; }
    public string ExternalId { get; set; }
    public int? SeasonNumber { get; set; }
    public List<EpisodeDto> Episodes { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Description { get; set; }
}