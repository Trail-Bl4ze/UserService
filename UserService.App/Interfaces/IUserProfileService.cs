using UserService.App.Models;

namespace UserService.App.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileDto> AddUserProfileAsync(UserProfileDto userProfileDto);
    Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UserProfileDto updateDto);
    Task<UserProfileDto?> GetUserProfileAsync(Guid userId);
}
