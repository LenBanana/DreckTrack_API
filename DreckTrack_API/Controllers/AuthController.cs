using AutoMapper;
using DreckTrack_API.Encryption;
using DreckTrack_API.Models.Dto;
using DreckTrack_API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DreckTrack_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, IMapper mapper)
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
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized();

        var token = JwtGeneration.GenerateJwtToken(user, configuration, model.RememberMe);
        if (token == null)
            return StatusCode(500);

        return Ok(new { token });
    }
}
