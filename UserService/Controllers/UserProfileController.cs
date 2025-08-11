using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UserService.App.Interfaces;
using UserService.App.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize] // Требует аутентификации
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

    #region check_tocken
    [Authorize]
    [HttpGet("check-context")]
    public IActionResult CheckContext()
    {
        return Ok(new
        {
            User.Identity.IsAuthenticated,
            User.Identity.Name, // Должно быть "7834e73d-4f89-42e4-ad3c-1b5f162c8e23"
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    [AllowAnonymous]
    [HttpGet("validate-token")]
    public IActionResult ValidateTokenManually()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("12345678901234567890123456789012"));
        
        try
        {
            var validator = new JwtSecurityTokenHandler();
            var principal = validator.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = "trailblaze",
                ValidateAudience = true,
                ValidAudience = "trailblaze",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return Ok(new {
                IsValid = true,
                Claims = principal.Claims.Select(c => new { c.Type, c.Value })
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpGet("debug")]
    public IActionResult Debug()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        var user = HttpContext.User.Identity?.IsAuthenticated;
        return Ok(new { 
            Header = authHeader,
            IsAuthenticated = user,
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
    #endregion

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