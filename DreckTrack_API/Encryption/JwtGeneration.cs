using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DreckTrack_API.Models.Entities;
using Microsoft.IdentityModel.Tokens;

namespace DreckTrack_API.Encryption;

public static class JwtGeneration
{
    public static (string? AccessToken, string? RefreshToken) GenerateTokens(ApplicationUser user, IConfiguration configuration)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = configuration["JwtSettings:Secret"];
            var accessTokenDuration = configuration["JwtSettings:ShortLivedTokenDurationInHours"];
            var refreshTokenDuration = configuration["JwtSettings:LongLivedTokenDurationInDays"];
            var issuer = configuration["JwtSettings:Issuer"];
            var audience = configuration["JwtSettings:Audience"];
            
            if (jwtKey == null || accessTokenDuration == null || refreshTokenDuration == null)
                return (null, null);
            
            var key = Encoding.ASCII.GetBytes(jwtKey);

            // **Access Token**
            var accessTokenExpiration = DateTime.UtcNow.AddHours(int.Parse(accessTokenDuration));
            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email ?? throw new InvalidOperationException())
                }),
                Expires = accessTokenExpiration,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var accessToken = tokenHandler.CreateToken(accessTokenDescriptor);
            var accessTokenString = tokenHandler.WriteToken(accessToken);

            // **Refresh Token**
            var refreshToken = GenerateRefreshToken();

            return (accessTokenString, refreshToken);
        }
        catch
        {
            return (null, null);
        }
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

