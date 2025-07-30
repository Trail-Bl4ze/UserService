using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserService.App.Interfaces;
using UserService.App.Models;
using UserService.Domain;
using UserService.Domain.Entities;

namespace UserService.App.Services;

public class UserProfileService : IUserProfileService
{
    private readonly UserDbContext FContext;
    private readonly IConfiguration FConfiguration;

    public UserProfileService(UserDbContext context, IConfiguration configuration)
    {
        FContext = context;
        FConfiguration = configuration;
    }

    public async Task<UserProfile> AddUserProfileAsync(UserProfileDto userProfileDto)
    {
        var user = await FContext.Users.FirstOrDefaultAsync(u => u.Id == userProfileDto.UserId);
        if (user == null)
            throw new Exception("Пользователь не найден");

        var profile = userProfileDto.Adapt<UserProfile>();

        await FContext.UserProfiles.AddAsync(profile);
        await FContext.SaveChangesAsync();

        return profile;
    }

    public async Task<UserProfile> UpdateUserProfileAsync(Guid userId, UserProfileDto updateDto)
    {
        var existingProfile = await FContext.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (existingProfile == null)
            throw new KeyNotFoundException("Профиль не найден");

        updateDto.Adapt(existingProfile);

        FContext.UserProfiles.Update(existingProfile);
        await FContext.SaveChangesAsync();

        return existingProfile;
    }
    
    public async Task<UserProfile?> GetUserProfileAsync(Guid userId)
    {
        return await FContext.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }
}