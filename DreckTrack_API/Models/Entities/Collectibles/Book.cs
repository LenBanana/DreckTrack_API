using DreckTrack_API.Models.Enums;

namespace DreckTrack_API.Models.Entities.Collectibles;

public class Book : CollectibleItem
{
    public List<string>? Authors { get; set; }
    public string? Publisher { get; set; }
    public int PageCount { get; set; }
    public int CurrentPage { get; set; }
    public BookFormat? Format { get; set; }
    
    public Book()
    {
        ItemType = "Book";
    }
}
