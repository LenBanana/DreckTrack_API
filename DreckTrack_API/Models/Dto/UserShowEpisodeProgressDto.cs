namespace DreckTrack_API.Models.Dto;

public class UserShowEpisodeProgressDto
{
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public bool Watched { get; set; }
    public DateTime? WatchedOn { get; set; }
}
