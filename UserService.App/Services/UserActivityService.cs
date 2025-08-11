using Amazon.S3;
using Mapster;
using Microsoft.AspNetCore.Http;
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
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _serviceUrl;

    public UserActivityService(UserDbContext context, IConfiguration configuration)
    {
        FContext = context;
        
        // Конфигурация для TimeWeb Cloud
        _bucketName = configuration["TimeWeb:BucketName"];
        _serviceUrl = configuration["TimeWeb:ServiceURL"];
        
        // Настройка S3 клиента для TimeWeb Cloud
        var config = new AmazonS3Config
        {
            ServiceURL = _serviceUrl,
            ForcePathStyle = true
        };
        
        _s3Client = new AmazonS3Client(
            configuration["TimeWeb:AccessKey"],
            configuration["TimeWeb:SecretKey"],
            config
        );
    }

    public async Task<UserActivityDTO> AddUserActivityAsync(UserActivityDTO userActivityDto)
    {
        var user = await FContext.Users.FirstOrDefaultAsync(u => u.Id == userActivityDto.UserId);
        if (user == null)
            throw new Exception("Пользователь не найден");

        var imageUrls = new List<string>();
        foreach (var imageFile in userActivityDto.ImageFiles)
        {
            var imageUrl = await UploadFileToTimeWebAsync(imageFile);
            imageUrls.Add(imageUrl);
        }

        var activity = new UserActivity
        {
            Id = Guid.NewGuid(),
            UserId = userActivityDto.UserId,
            Latitude = userActivityDto.Latitude,
            Longitude = userActivityDto.Longitude,
            Title = userActivityDto.Title,
            Text = userActivityDto.Text,
            Images = imageUrls,
        };

        await FContext.UserActivities.AddAsync(activity);
        await FContext.SaveChangesAsync();

        return new UserActivityDTO
        {
            Id = activity.Id,
            UserId = activity.UserId,
            Latitude = activity.Latitude,
            Longitude = activity.Longitude,
            Title = activity.Title,
            Text = activity.Text
        };
    }

    private async Task<string> UploadFileToTimeWebAsync(IFormFile file)
    {
        var fileKey = $"user-activities/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        using (var stream = file.OpenReadStream())
        {
            var request = new Amazon.S3.Model.PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileKey,
                InputStream = stream,
                ContentType = file.ContentType,
                CannedACL = S3CannedACL.PublicRead
            };

            await _s3Client.PutObjectAsync(request);
        }

        // Формируем URL в зависимости от настроек TimeWeb Cloud
        return $"{_serviceUrl}/{_bucketName}/{fileKey}";
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
        return FContext.UserActivities
            .Where(p => p.UserId == userId).Adapt<List<UserActivityDTO>>();
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