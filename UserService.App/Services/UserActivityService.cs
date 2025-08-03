using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserService.App.Interfaces;
using UserService.App.Models;
using UserService.Domain;
using UserService.Domain.Entities;

namespace UserService.App.Services;

public class UserActivityService : IUserActivityService
{
    private readonly UserDbContext FContext;
    private readonly IConfiguration FConfiguration;

    public UserActivityService(UserDbContext context, IConfiguration configuration)
    {
        FContext = context;
        FConfiguration = configuration;
    }

    public async Task<UserActivityDTO> AddUserActivitysync(UserActivityDTO userActivity)
    {
        var user = await FContext.Users.FirstOrDefaultAsync(u => u.Id == userActivity.UserId);
        if (user == null)
            throw new Exception("Пользователь не найден");

        var activity = userActivity.Adapt<UserActivity>();

        await FContext.UserActivities.AddAsync(activity);
        await FContext.SaveChangesAsync();

        return activity.Adapt<UserActivityDTO>();
    }

    public async Task<UserActivityDTO> UpdateUserActivityAsync(Guid userId, UserActivityDTO updateDto)
    {
        var existingActivity = await FContext.UserActivities
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (existingActivity == null)
            throw new KeyNotFoundException("Активность не найдена");

        updateDto.Adapt(existingActivity);

        FContext.UserActivities.Update(existingActivity);
        await FContext.SaveChangesAsync();

        return existingActivity.Adapt<UserActivityDTO>();
    }

    public async Task<List<UserActivityDTO>> GetAllUserActivitiesAsync(Guid userId)
    {
        return (await FContext.UserActivities
            .AllAsync(p => p.UserId == userId)).Adapt<List<UserActivityDTO>>();
    }

    public async Task<int> DeleteUserActivityAsync(Guid id)
    {
        var existingActivity = await FContext.UserActivities
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existingActivity == null)
            throw new KeyNotFoundException("Активность не найдена");

        FContext.UserActivities.Remove(existingActivity);
        int affectedRows = await FContext.SaveChangesAsync();
        
        return affectedRows;
    }
}