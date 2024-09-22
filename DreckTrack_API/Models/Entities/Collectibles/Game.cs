namespace DreckTrack_API.Models.Entities.Collectibles;

public class Game : CollectibleItem
{
    public string? Platform { get; set; }
    
    public Game()
    {
        ItemType = "Game";
    }
}