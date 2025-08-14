using UserService.App.Models;

namespace UserService.App.Interfaces;

public interface IUserActivityService
{
    Task<UserActivityResponse> AddUserActivityAsync(UserActivityRequest userActivity);
    Task<UserActivityResponse> UpdateUserActivityAsync(Guid userId, UserActivityRequest updateDto);
    Task<List<UserActivityResponse>> GetAllUserActivitiesAsync(Guid userId);
    Task<int> DeleteUserActivityAsync(Guid id);
}
