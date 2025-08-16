using UserService.App.Models;

namespace UserService.App.Interfaces;

public interface IUserActivityService
{
    Task<bool> AddUserActivityAsync(UserActivityRequest userActivity);
    Task<List<UserActivityResponse>> GetAllUserActivitiesAsync(Guid userId);
}
