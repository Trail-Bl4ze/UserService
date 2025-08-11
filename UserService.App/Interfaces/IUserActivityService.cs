using UserService.App.Models;

namespace UserService.App.Interfaces;

public interface IUserActivityService
{
    Task<UserActivityDTO> AddUserActivityAsync(UserActivityDTO userActivity);
    Task<UserActivityDTO> UpdateUserActivityAsync(Guid userId, UserActivityDTO updateDto);
    Task<List<UserActivityDTO>> GetAllUserActivitiesAsync(Guid userId);
    Task<int> DeleteUserActivityAsync(Guid id);
}
