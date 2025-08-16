using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.App.Interfaces;
using UserService.App.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Требует аутентификации
public class UserActivitiesController : ControllerBase
{
    private readonly IUserActivityService FActivityService;
    private readonly IHttpContextAccessor FHttpContextAccessor;

    public UserActivitiesController(
        IUserActivityService activityService, 
        IHttpContextAccessor httpContextAccessor)
    {
        FActivityService = activityService;
        FHttpContextAccessor = httpContextAccessor;
    }

    // Получение ID пользователя из токена
    private Guid GetUserIdFromToken()
    {
        var userIdClaim = FHttpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }
        return userId;
    }

    [HttpPost]
    public async Task<IActionResult> AddActivity([FromForm] UserActivityRequest activityDto)
    {
        try
        {
            var userId = GetUserIdFromToken();
            activityDto.UserId = userId;

            var result = await FActivityService.AddUserActivityAsync(activityDto);
            if (!result)
            {
                return BadRequest();
            }
        
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllActivities()
    {
        try
        {
            var userId = GetUserIdFromToken();
            var activities = await FActivityService.GetAllUserActivitiesAsync(userId);
            return Ok(activities);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}