namespace DreckTrack_API.Models.Dto.Items.Show;

public class ShowDto : CollectibleItemDto
{
    public List<SeasonDto> Seasons { get; set; }
}
