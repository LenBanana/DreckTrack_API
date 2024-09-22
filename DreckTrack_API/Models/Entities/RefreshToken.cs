namespace DreckTrack_API.Models.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; } = Guid.Empty;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
    
    // Navigation Property
    public ApplicationUser User { get; set; }
}
