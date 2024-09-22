using DreckTrack_API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DreckTrack_API.Controllers.AuthFilter;

public class UserExistenceFilter(UserManager<ApplicationUser> userManager) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = await userManager.GetUserAsync(context.HttpContext.User);

        // If the user doesn't exist, return Unauthorized
        if (user == null)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}