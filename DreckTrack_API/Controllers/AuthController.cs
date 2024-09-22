using AutoMapper;
using DreckTrack_API.Encryption;
using DreckTrack_API.Models.Dto;
using DreckTrack_API.Models.Entities;
using DreckTrack_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DreckTrack_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration,
    IMapper mapper,
    RefreshTokenService refreshTokenService)
    : ControllerBase
{
    private readonly IMapper _mapper = mapper;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            DisplayName = model.DisplayName
        };

        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto model)
    {
        var jwtRefreshTokenDuration = configuration["JwtSettings:LongLivedTokenDurationInDays"];
        var domain = configuration["Cors:Domain"];
        if (jwtRefreshTokenDuration == null || domain == null)
            return StatusCode(500);
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized();

        var tokens = JwtGeneration.GenerateTokens(user, configuration);
        if (tokens.AccessToken == null || tokens.RefreshToken == null)
            return StatusCode(500);

        // Save the refresh token
        var refreshTokenDuration = int.Parse(jwtRefreshTokenDuration);
        await refreshTokenService.SaveRefreshTokenAsync(user.Id, tokens.RefreshToken, refreshTokenDuration);

        // Set the refresh token in an HTTP-only cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Ensure HTTPS
            SameSite = SameSiteMode.None,
            Domain = domain,
            Expires = DateTime.UtcNow.AddDays(refreshTokenDuration)
        };
        Response.Cookies.Append("refreshToken", tokens.RefreshToken, cookieOptions);

        return Ok(new { token = tokens.AccessToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var jwtRefreshTokenDuration = configuration["JwtSettings:LongLivedTokenDurationInDays"];
        if (jwtRefreshTokenDuration == null)
            return StatusCode(500);
        // Retrieve refresh token from HTTP-only cookie
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return Unauthorized(new { message = "Refresh token not found." });

        var existingRefreshToken = await refreshTokenService.GetRefreshTokenAsync(refreshToken);
        if (existingRefreshToken == null)
            return Unauthorized(new { message = "Invalid or expired refresh token." });

        var user = existingRefreshToken.User;

        // Optionally, revoke the old refresh token and generate a new one
        await refreshTokenService.RevokeRefreshTokenAsync(existingRefreshToken);

        // Generate new tokens
        var tokens = JwtGeneration.GenerateTokens(user, configuration);
        if (tokens.AccessToken == null || tokens.RefreshToken == null)
            return StatusCode(500);

        // Save the new refresh token
        var refreshTokenDuration = int.Parse(jwtRefreshTokenDuration);
        await refreshTokenService.SaveRefreshTokenAsync(user.Id, tokens.RefreshToken, refreshTokenDuration);

        // Set the new refresh token in an HTTP-only cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(refreshTokenDuration)
        };
        Response.Cookies.Append("refreshToken", tokens.RefreshToken, cookieOptions);

        return Ok(new 
        { 
            accessToken = tokens.AccessToken 
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Retrieve refresh token from HTTP-only cookie
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return BadRequest(new { message = "Refresh token not found." });

        var existingRefreshToken = await refreshTokenService.GetRefreshTokenAsync(refreshToken);
        if (existingRefreshToken != null)
        {
            await refreshTokenService.RevokeRefreshTokenAsync(existingRefreshToken);
        }

        // Remove the refresh token cookie
        Response.Cookies.Delete("refreshToken");

        return Ok(new { message = "Logged out successfully." });
    }
}

