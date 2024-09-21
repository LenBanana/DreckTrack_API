namespace DreckTrack_API.Models.Entities.Collectibles;

public class Show : CollectibleItem
{
    public List<Season> Seasons { get; set; }
    
    public Show()
    {
        ItemType = "Show";
    }
}