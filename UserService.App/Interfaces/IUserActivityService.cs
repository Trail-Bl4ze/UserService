using UserService.App.Models;

namespace UserService.App.Interfaces;

public interface IUserActivityService
{
    Task<UserActivityResponse> AddUserActivityAsync(UserActivityRequest userActivity);
    Task<UserActivityRequest> UpdateUserActivityAsync(Guid userId, UserActivityRequest updateDto);
    Task<List<UserActivityRequest>> GetAllUserActivitiesAsync(Guid userId);
    Task<int> DeleteUserActivityAsync(Guid id);
}
