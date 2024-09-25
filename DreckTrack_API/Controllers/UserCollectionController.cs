using System.Security.Claims;
using AutoMapper;
using DreckTrack_API.Controllers.AuthFilter;
using DreckTrack_API.Controllers.Utilities;
using DreckTrack_API.Database;
using DreckTrack_API.Models.Dto;
using DreckTrack_API.Models.Dto.Items;
using DreckTrack_API.Models.Entities;
using DreckTrack_API.Models.Entities.Collectibles.Items;
using DreckTrack_API.Models.Entities.Collectibles.Items.Show;
using DreckTrack_API.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DreckTrack_API.Controllers;

[ServiceFilter(typeof(UserExistenceFilter))]
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserCollectionController(
    ApplicationDbContext context,
    IMapper mapper,
    UserManager<ApplicationUser> userManager)
    : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    private (int pageNumber, int pageSize) ValidatePageValues(int pageNumber, int pageSize, int totalItems)
    {
        pageSize = pageSize > 100 ? 100 : pageSize < 1 ? 1 : pageSize;
        pageNumber = pageNumber < 1 ? 1 :
            pageNumber > (totalItems / pageSize) + 1 ? (totalItems / pageSize) + 1 : pageNumber;
        return (pageNumber, pageSize);
    }

    // GET: api/UserCollection
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserCollectibleItemDto>>> GetUserCollection(
        [FromQuery] string? itemType,
        [FromQuery] CollectibleStatus? status,
        [FromQuery] string? filterTerm,
        [FromQuery] string? orderBy,
        [FromQuery] string? orderDirection = "asc",
        [FromQuery] ICollection<string>? excludeExternalIds = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        // Retrieve and validate user ID
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var guidUserId))
        {
            return Unauthorized();
        }

        // Initialize the query with AsNoTracking for read-only optimization
        var query = context.UserCollectibleItems
            .AsNoTracking()
            .Include(uci => uci.CollectibleItem.ExternalIds)
            .Where(uci => uci.UserId == guidUserId);
        
        // Exclude items with external IDs
        if (excludeExternalIds is { Count: > 0 })
        {
            // Check the external IDs identifier, CollectibleItem.ExternalIds is a collection of ExternalId
            query = query.Where(uci => uci.CollectibleItem.ExternalIds != null && !uci.CollectibleItem.ExternalIds
                .Any(externalId => excludeExternalIds.Contains(externalId.Identifier)));
        }

        // Apply filters
        if (!string.IsNullOrEmpty(itemType))
        {
            query = query.Where(uci => uci.CollectibleItem.ItemType == itemType);
        }

        if (status.HasValue)
        {
            query = query.Where(uci => uci.Status == status.Value);
        }

        if (!string.IsNullOrEmpty(filterTerm))
        {
            // Use case-insensitive comparison without ToLower()
            query = query.Where(uci =>
                EF.Functions.Like(uci.CollectibleItem.Title, $"%{filterTerm}%") ||
                EF.Functions.Like(uci.CollectibleItem.Description, $"%{filterTerm}%"));
        }

        // Conditional inclusion for "Show" itemType
        if (itemType == "Show")
        {
            query = query
                .Include(uci => ((Show)uci.CollectibleItem).Seasons)
                .ThenInclude(season => season.Episodes);
        }


        // Normalize orderDirection
        orderDirection = orderDirection?.ToLower();
        if (orderDirection != "asc" && orderDirection != "desc")
        {
            orderDirection = "asc";
        }

        // Get ordering options based on itemType
        var orderingOptions = UserCollectibleItemOrderingOptions.GetOptions(itemType);

        // Apply ordering
        if (!string.IsNullOrEmpty(orderBy))
        {
            orderBy = orderBy.Replace(" ", ""); // Remove spaces for key comparison
            var selectedOrder = orderingOptions
                .FirstOrDefault(o => string.Equals(o.Key, orderBy, StringComparison.OrdinalIgnoreCase));

            if (selectedOrder != null)
            {
                // Apply ascending or descending order based on orderDirection
                query = orderDirection == "desc"
                    ? query.OrderByDescending(selectedOrder.OrderExpression)
                    : query.OrderBy(selectedOrder.OrderExpression);
            }
            else
            {
                // Default ordering if orderBy is invalid
                query = query.OrderBy(uci => uci.DateAdded);
            }
        }
        else
        {
            // Default ordering if none specified
            query = query.OrderBy(uci => uci.DateAdded);
        }

        // Get total count for pagination
        var totalItems = await query.CountAsync();

        // Validate and adjust pagination parameters
        (pageNumber, pageSize) = ValidatePageValues(pageNumber, pageSize, totalItems);

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Map to DTO
        var itemsDto = mapper.Map<List<UserCollectibleItemDto>>(items);

        // Extract available ordering display names
        var orderingValues = orderingOptions.Select(o => o.DisplayName).ToList();

        // Return paged result
        var pagedResult = new PagedResult<UserCollectibleItemDto>
        {
            TotalItems = totalItems,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = itemsDto,
            OrderFields = orderingValues,
            CurrentOrderBy = orderBy,
            CurrentOrderDirection = orderDirection
        };

        return Ok(pagedResult);
    }

    // GET: api/UserCollection/{itemId}
    [HttpGet("{itemId:guid}")]
    public async Task<ActionResult<UserCollectibleItemDto>> GetUserCollectionItem(Guid itemId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var guidUserId = Guid.Parse(userId);

        var item = await context.UserCollectibleItems
            .Include(uci => uci.CollectibleItem)
            .FirstOrDefaultAsync(uci => uci.UserId == guidUserId && uci.CollectibleItemId == itemId);

        if (item == null)
        {
            return NotFound();
        }

        // Include seasons and episodes for shows if itemType is "Show"
        var itemType = item.CollectibleItem.ItemType;
        if (itemType == "Show")
        {
            await context.Entry(item.CollectibleItem)
                .Collection(uci => ((Show)uci).Seasons)
                .Query()
                .Include(season => season.Episodes.OrderBy(episode => episode.EpisodeNumber))
                .OrderBy(season => season.SeasonNumber)
                .LoadAsync();
        }

        var itemDto = mapper.Map<UserCollectibleItemDto>(item);
        return Ok(itemDto);
    }
    
    // An endpoint that takes a list of external IDs and returns only the external IDs that are not in the user's collection
    // GET: api/UserCollection/external-ids
    [HttpPost("external-ids")]
    public async Task<ActionResult<ICollection<string>>> GetExternalIdsNotInCollection([FromBody] ICollection<string> externalIds)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var guidUserId = Guid.Parse(userId);

        var existingExternalIds = await context.UserCollectibleItems
            .Where(uci => uci.UserId == guidUserId && uci.CollectibleItem.ExternalIds != null)
            .SelectMany(uci => uci.CollectibleItem.ExternalIds!)
            .Select(externalId => externalId.Identifier)
            .ToListAsync();

        var externalIdsNotInCollection = externalIds.Except(existingExternalIds).ToList();
        return Ok(externalIdsNotInCollection);
    }

    // POST: api/UserCollection
    [HttpPost]
    public async Task<IActionResult> AddItemToCollection([FromBody] AddUserCollectibleItemDto itemDto)
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(nameIdentifier ?? throw new InvalidOperationException());
        var existingItem = await context.UserCollectibleItems
            .FirstOrDefaultAsync(uci => uci.UserId == userId && uci.CollectibleItem.Id == itemDto.CollectibleItem.Id);

        if (existingItem != null)
        {
            return BadRequest("Item already exists in your collection.");
        }

        var userCollectibleItem = mapper.Map<UserCollectibleItem>(itemDto);
        userCollectibleItem.UserId = userId;
        userCollectibleItem.DateAdded = DateTime.UtcNow;

        context.UserCollectibleItems.Add(userCollectibleItem);
        await context.SaveChangesAsync();

        return Ok();
    }

    // POST: api/UserCollection/multiple
    [HttpPost("multiple")]
    public async Task<IActionResult> AddMultipleItemsToCollection([FromBody] List<AddUserCollectibleItemDto> itemsDto)
    {
        var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = Guid.Parse(nameIdentifier ?? throw new InvalidOperationException());

        var existingItems = await context.UserCollectibleItems
            .Where(uci => uci.UserId == userId)
            .Select(uci => uci.CollectibleItemId)
            .ToListAsync();

        var itemsToAdd = itemsDto
            .Where(itemDto => !existingItems.Contains(itemDto.CollectibleItem.Id))
            .Select(itemDto =>
            {
                var userCollectibleItem = mapper.Map<UserCollectibleItem>(itemDto);
                userCollectibleItem.UserId = userId;
                userCollectibleItem.DateAdded = DateTime.UtcNow;
                return userCollectibleItem;
            });

        context.UserCollectibleItems.AddRange(itemsToAdd);
        await context.SaveChangesAsync();

        return Ok();
    }

    // PUT: api/UserCollection/5
    [HttpPut("{itemId:guid}")]
    public async Task<IActionResult> UpdateItemStatus(Guid itemId, [FromBody] UserCollectibleItemDto itemDto)
    {
        var userId =
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException());
        var userCollectibleItem = await context.UserCollectibleItems
            .Include(userCollectibleItem => userCollectibleItem.CollectibleItem)
            .ThenInclude(ci => ((Show)ci).Seasons)
            .ThenInclude(s => s.Episodes)
            .FirstOrDefaultAsync(uci => uci.UserId == userId && uci.CollectibleItemId == itemId);

        if (userCollectibleItem == null)
        {
            return NotFound();
        }

        // Update properties
        userCollectibleItem.UpdatedAt = DateTime.UtcNow;
        userCollectibleItem.CollectibleItem.UpdatedAt = DateTime.UtcNow;
        mapper.Map(itemDto, userCollectibleItem);

        await context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/UserCollection/multiple
    [HttpPut("multiple")]
    public async Task<IActionResult> UpdateMultipleItemsStatus([FromBody] List<UserCollectibleItemDto> itemsDto)
    {
        var userId =
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException());
        var userCollectibleItems = await context.UserCollectibleItems
            .Include(userCollectibleItem => userCollectibleItem.CollectibleItem)
            .ThenInclude(ci => ((Show)ci).Seasons)
            .ThenInclude(s => s.Episodes)
            .Where(uci => uci.UserId == userId)
            .ToListAsync();

        if (userCollectibleItems.Count == 0)
        {
            return NotFound();
        }

        foreach (var itemDto in itemsDto)
        {
            var userCollectibleItem = userCollectibleItems
                .FirstOrDefault(uci => uci.CollectibleItemId == itemDto.CollectibleItemId);

            if (userCollectibleItem == null)
            {
                continue;
            }

            // Update properties
            userCollectibleItem.UpdatedAt = DateTime.UtcNow;
            userCollectibleItem.CollectibleItem.UpdatedAt = DateTime.UtcNow;
            mapper.Map(itemDto, userCollectibleItem);
        }

        await context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/UserCollection/5
    [HttpDelete("{itemId:guid}")]
    public async Task<IActionResult> RemoveItemFromCollection(Guid itemId)
    {
        var userId =
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException());
        var userCollectibleItem = await context.UserCollectibleItems
            .Include(userCollectibleItem => userCollectibleItem.CollectibleItem)
            .FirstOrDefaultAsync(uci => uci.UserId == userId && uci.CollectibleItemId == itemId);

        if (userCollectibleItem == null)
        {
            return NotFound();
        }

        context.UserCollectibleItems.Remove(userCollectibleItem);
        context.CollectibleItems.Remove(userCollectibleItem.CollectibleItem);
        await context.SaveChangesAsync();

        return NoContent();
    }
}