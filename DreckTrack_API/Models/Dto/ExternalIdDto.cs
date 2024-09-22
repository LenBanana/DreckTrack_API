namespace DreckTrack_API.Models.Dto;

public class ExternalIdDto
{
    public Guid? Id { get; set; }
    public Guid CollectibleItemId { get; set; }
    public string Source { get; set; }
    public string Identifier { get; set; }
}
