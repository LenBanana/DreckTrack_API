using DreckTrack_API.Models.Enums;

namespace DreckTrack_API.Models.Dto.Items;

public class AddUserCollectibleItemDto
{
    public CollectibleStatus Status { get; set; }
    public int? UserRating { get; set; }
    public string Notes { get; set; }
    public DateTime? DateStarted { get; set; }

    public DateTime? DateCompleted { get; set; }
    public CollectibleItemDto CollectibleItem { get; set; }
}

public class UserCollectibleItemDto : AddUserCollectibleItemDto
{
    public Guid CollectibleItemId { get; set; }
}