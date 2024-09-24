namespace DreckTrack_API.Models.Dto.Items.Game;

public class GameDto : CollectibleItemDto
{
    public string? Platform { get; set; }
    public double TimePlayed { get; set; }
}