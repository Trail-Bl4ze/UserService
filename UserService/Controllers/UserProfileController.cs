using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.App.Interfaces;
using UserService.App.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Требует аутентификации
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService FProfileService;
    private readonly IHttpContextAccessor FHttpContextAccessor;

    public UserProfileController(
        IUserProfileService profileService, 
        IHttpContextAccessor httpContextAccessor)
    {
        FProfileService = profileService;
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

    // Добавление активности
    [HttpPost]
    public async Task<IActionResult> AddProfile([FromBody] UserProfileDto profileDto)
    {
        try
        {
            var userId = GetUserIdFromToken();
            profileDto.UserId = userId;
            
            var result = await FProfileService.AddUserProfileAsync(profileDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UserProfileDto updateDto)
    {
        try
        {
            var userId = GetUserIdFromToken();
            updateDto.UserId = userId;
            
            var result = await FProfileService.UpdateUserProfileAsync(userId, updateDto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Получение всех активностей пользователя
    [HttpGet]
    public async Task<IActionResult> GetProfileAsync()
    {
        try
        {
            var userId = GetUserIdFromToken();
            var activities = await FProfileService.GetUserProfileAsync(userId);
            return Ok(activities);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}