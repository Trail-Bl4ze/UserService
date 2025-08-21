using System.Text.Json;
using Amazon.S3;
using Grpc.Core;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UserService.App.BackgroundJobs;
using UserService.App.Interfaces;
using UserService.App.Models;
using UserService.Domain;
using UserService.Domain.Entities;
using ActivityService.Grpc;

namespace UserService.App.Services;

public class UserActivityService : IUserActivityService
{
    private readonly UserDbContext FContext;
    private readonly KafkaProducerService _kafkaProducer;
    private readonly ILogger<UserActivityService> _logger;
    private readonly ActivitiesGrpc.ActivitiesGrpcClient _grpcClient;
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _serviceUrl;

    public UserActivityService(UserDbContext context, ActivitiesGrpc.ActivitiesGrpcClient grpcClient, KafkaProducerService kafkaProducer, ILogger<UserActivityService> logger, IConfiguration configuration)
    {
        _grpcClient = grpcClient;
        FContext = context;

        _kafkaProducer = kafkaProducer;
        _logger = logger;

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

    public async Task<bool> AddUserActivityAsync(UserActivityRequest request)
    {
        // var user = await FContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        // if (user == null)
        //     throw new Exception("Пользователь не найден");

        var imageUrls = new List<string>();
        foreach (var imageFile in request.ImageFiles)
        {
            var imageUrl = await UploadFileToTimeWebAsync(imageFile);
            imageUrls.Add(imageUrl);
        }

        var messageObject = new UserActivityKafkaMessage()
        {
            UserId = (Guid)request.UserId,
            Title = request.Title,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            ImageFiles = imageUrls
        };
        try
        {
            var message = JsonSerializer.Serialize(messageObject);

            await _kafkaProducer.ProduceAsync(message);

            _logger.LogInformation("Activity sent to Kafka: {Message}", message);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send activity to Kafka");
            return false;
        }
    }


    public async Task<List<UserActivityResponse>> GetAllUserActivitiesAsync(Guid userId)
    {
        try
        {
            var request = new UserActivitiesRequest { UserId = userId.ToString() };
            var response = await _grpcClient.GetUserActivitiesAsync(request);
            
            return response.Activities
                .Select(FromGrpcResponse)
                .ToList();
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return new List<UserActivityResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC call failed");
            throw;
        }
    }

    // public async Task<List<UserActivityResponse>> GetActivityByIdAsync(Guid id)
    // {
    //     try
    //     {
    //         var request = new ActivityByIdRequest { Id = id.ToString() };
    //         var response = await _grpcClient.GetActivityByIdAsync(request);
            
    //         return new List<UserActivityResponse> { FromGrpcResponse(response) };
    //     }
    //     catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
    //     {
    //         return new List<UserActivityResponse>();
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "gRPC call failed");
    //         throw;
    //     }
    // }

    private static UserActivityResponse FromGrpcResponse(ActivityResponse grpcResponse)
    {
        return new UserActivityResponse
        {
            Id = Guid.Parse(grpcResponse.Id),
            UserId = Guid.Parse(grpcResponse.UserId),
            Title = grpcResponse.Title,
            Description = grpcResponse.Description,
            Latitude = grpcResponse.Latitude,
            Longitude = grpcResponse.Longitude,
            ImagesUrls = grpcResponse.ImagesUrls.ToList()
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
        return $"{_serviceUrl}/{_bucketName}/{fileKey}";
    }
}