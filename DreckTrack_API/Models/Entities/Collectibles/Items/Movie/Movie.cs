namespace DreckTrack_API.Models.Entities.Collectibles.Items.Movie;

public class Movie : CollectibleItem
{
    public double? Duration { get; set; }
    
    public Movie()
    {
        ItemType = "Movie";
    }
}