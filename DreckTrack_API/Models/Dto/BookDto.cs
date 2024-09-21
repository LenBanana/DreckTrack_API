using DreckTrack_API.Models.Enums;

namespace DreckTrack_API.Models.Dto;

public class BookDto : CollectibleItemDto
{
    public List<string> Authors { get; set; }
    public string Publisher { get; set; }
    public int? PageCount { get; set; }
    public int? CurrentPage { get; set; }
    public BookFormat? Format { get; set; }
}
