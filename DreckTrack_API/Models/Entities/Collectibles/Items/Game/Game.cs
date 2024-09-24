namespace DreckTrack_API.Models.Entities.Collectibles.Items.Game;

public class Game : CollectibleItem
{
    public string? Platform { get; set; }
    public double TimePlayed { get; set; }
    
    public Game()
    {
        ItemType = "Game";
    }
}