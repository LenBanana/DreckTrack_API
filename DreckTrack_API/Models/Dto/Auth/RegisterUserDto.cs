namespace DreckTrack_API.Models.Dto.Auth;

public class RegisterUserDto
{
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}