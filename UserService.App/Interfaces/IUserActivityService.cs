using UserService.App.Models;

namespace UserService.App.Interfaces;

public interface IUserActivityService
{
    Task<UserActivityDTO> AddUserActivitysync(UserActivityDTO userActivity);
    Task<UserActivityDTO> UpdateUserProfileAsync(Guid userId, UserActivityDTO updateDto);
    Task<List<UserActivityDTO>> GetAllUserActivitiesAsync(Guid userId);
    Task<int> DeleteUserActivityAsync(Guid id);
}
