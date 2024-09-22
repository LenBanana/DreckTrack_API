using DreckTrack_API.Controllers.AuthFilter;
using DreckTrack_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DreckTrack_API.Controllers;

[ServiceFilter(typeof(UserExistenceFilter))]
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RecommendationController(
    IHttpClientFactory httpClientFactory,
    IOptions<TestDiveSettings> testDiveSettings)
    : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetRecommendations(
        [FromQuery] string query,
        [FromQuery] string type
    )
    {
        var client = httpClientFactory.CreateClient("TestDiveClient");

        // Build the query parameters
        var queryParams = new Dictionary<string, string>
        {
            { "k", testDiveSettings.Value.ApiKey },
            { "q", query },
            { "type", type },
            { "info", "1"}
        };

        // Construct the query string
        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        var requestUrl = $"similar?{queryString}";

        try
        {
            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // Return the RAW JSON response directly
                return Content(content, "application/json");
            }

            // Optionally, you can handle different status codes here
            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorContent);
        }
        catch (Exception ex)
        {
            // Log the exception (not implemented here)
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}