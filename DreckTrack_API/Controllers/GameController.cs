using AutoMapper;
using DreckTrack_API.Controllers.AuthFilter;
using DreckTrack_API.Database;
using DreckTrack_API.Models;
using DreckTrack_API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DreckTrack_API.Controllers;

[ServiceFilter(typeof(UserExistenceFilter))]
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GameController(
    UserManager<ApplicationUser> userManager,
    IHttpClientFactory httpClientFactory,
    IOptions<RawgSettings> rawgSettings)
    : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetGames(
        [FromQuery] int? page,
        [FromQuery(Name = "page_size")] int? pageSize,
        [FromQuery] string search,
        [FromQuery(Name = "search_precise")] bool? searchPrecise,
        [FromQuery(Name = "search_exact")] bool? searchExact,
        [FromQuery(Name = "parent_platforms")] string? parentPlatforms,
        [FromQuery] string? platforms,
        [FromQuery] string? stores,
        [FromQuery] string? developers,
        [FromQuery] string? publishers,
        [FromQuery] string? genres,
        [FromQuery] string? tags,
        [FromQuery] string? creators,
        [FromQuery] string? dates,
        [FromQuery] string? updated,
        [FromQuery(Name = "platforms_count")] int? platformsCount,
        [FromQuery] string? metacritic,
        [FromQuery(Name = "exclude_collection")]
        int? excludeCollection,
        [FromQuery(Name = "exclude_additions")]
        bool? excludeAdditions,
        [FromQuery(Name = "exclude_parents")] bool? excludeParents,
        [FromQuery(Name = "exclude_game_series")]
        bool? excludeGameSeries,
        [FromQuery(Name = "exclude_stores")] string? excludeStores,
        [FromQuery] string? ordering
    )
    {
        var client = httpClientFactory.CreateClient("RawgClient");

        // Build the query parameters
        var queryParams = new Dictionary<string, string>
        {
            { "key", rawgSettings.Value.ApiKey }
        };

        if (page.HasValue) queryParams.Add("page", page.Value.ToString());
        if (pageSize.HasValue) queryParams.Add("page_size", pageSize.Value.ToString());
        if (!string.IsNullOrEmpty(search)) queryParams.Add("search", search);
        if (searchPrecise.HasValue) queryParams.Add("search_precise", searchPrecise.Value.ToString().ToLower());
        if (searchExact.HasValue) queryParams.Add("search_exact", searchExact.Value.ToString().ToLower());
        if (!string.IsNullOrEmpty(parentPlatforms)) queryParams.Add("parent_platforms", parentPlatforms);
        if (!string.IsNullOrEmpty(platforms)) queryParams.Add("platforms", platforms);
        if (!string.IsNullOrEmpty(stores)) queryParams.Add("stores", stores);
        if (!string.IsNullOrEmpty(developers)) queryParams.Add("developers", developers);
        if (!string.IsNullOrEmpty(publishers)) queryParams.Add("publishers", publishers);
        if (!string.IsNullOrEmpty(genres)) queryParams.Add("genres", genres);
        if (!string.IsNullOrEmpty(tags)) queryParams.Add("tags", tags);
        if (!string.IsNullOrEmpty(creators)) queryParams.Add("creators", creators);
        if (!string.IsNullOrEmpty(dates)) queryParams.Add("dates", dates);
        if (!string.IsNullOrEmpty(updated)) queryParams.Add("updated", updated);
        if (platformsCount.HasValue) queryParams.Add("platforms_count", platformsCount.Value.ToString());
        if (!string.IsNullOrEmpty(metacritic)) queryParams.Add("metacritic", metacritic);
        if (excludeCollection.HasValue) queryParams.Add("exclude_collection", excludeCollection.Value.ToString());
        if (excludeAdditions.HasValue)
            queryParams.Add("exclude_additions", excludeAdditions.Value.ToString().ToLower());
        if (excludeParents.HasValue) queryParams.Add("exclude_parents", excludeParents.Value.ToString().ToLower());
        if (excludeGameSeries.HasValue)
            queryParams.Add("exclude_game_series", excludeGameSeries.Value.ToString().ToLower());
        if (!string.IsNullOrEmpty(excludeStores)) queryParams.Add("exclude_stores", excludeStores);
        if (!string.IsNullOrEmpty(ordering)) queryParams.Add("ordering", ordering);

        // Construct the query string
        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

        var requestUrl = $"games?{queryString}";

        try
        {
            var response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // Return the RAW JSON response directly
                return Content(content, "application/json");
            }
            else
            {
                // Optionally, you can handle different status codes here
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            // Log the exception (not implemented here)
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}