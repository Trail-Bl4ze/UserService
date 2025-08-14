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

    public async Task<UserActivityResponse> AddUserActivityAsync(UserActivityRequest UserActivityRequest)
    {
        //TODO отправлять в сервис активностей
        return new UserActivityResponse();
    }

    public async Task<UserActivityResponse> UpdateUserActivityAsync(Guid userId, UserActivityRequest updateDto)
    {
        //TODO отправлять в сервис активностей
        return new UserActivityResponse();
    }

    public async Task<List<UserActivityResponse>> GetAllUserActivitiesAsync(Guid userId)
    {
        //TODO отправлять в сервис активностей
        return new List<UserActivityResponse>();
    }

    public async Task<int> DeleteUserActivityAsync(Guid id)
    {
        //TODO отправлять в сервис активностей
        return 0;
    }
}