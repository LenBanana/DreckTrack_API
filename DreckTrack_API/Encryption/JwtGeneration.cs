using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DreckTrack_API.Models.Entities;
using Microsoft.IdentityModel.Tokens;

namespace DreckTrack_API.Encryption;

public static class JwtGeneration
{
    public static string? GenerateJwtToken(ApplicationUser user, IConfiguration configuration, bool rememberMe)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = configuration["JwtSettings:Secret"];
            var shortLivedToken = configuration["JwtSettings:ShortLivedTokenDurationInHours"];
            var longLivedToken = configuration["JwtSettings:LongLivedTokenDurationInDays"];
            var issuer = configuration["JwtSettings:Issuer"];
            var audience = configuration["JwtSettings:Audience"];
            if (jwtKey == null || shortLivedToken == null || longLivedToken == null)
                return null;
            var key = Encoding.ASCII.GetBytes(jwtKey);

            // Retrieve token durations from configuration
            var shortLivedTokenDuration = int.Parse(shortLivedToken);
            var longLivedTokenDuration = int.Parse(longLivedToken);

            var tokenExpiration = rememberMe
                ? DateTime.UtcNow.AddDays(longLivedTokenDuration)
                : DateTime.UtcNow.AddHours(shortLivedTokenDuration);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email ?? throw new InvalidOperationException())
                }),
                Expires = tokenExpiration,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch
        {
            return null;
        }
    }
}
