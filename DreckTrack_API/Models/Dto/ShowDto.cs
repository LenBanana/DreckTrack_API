namespace DreckTrack_API.Models.Dto;

public class ShowDto : CollectibleItemDto
{
    public List<SeasonDto> Seasons { get; set; }
}
