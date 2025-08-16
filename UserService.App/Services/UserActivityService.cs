using System.Text.Json;
using Amazon.S3;
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

namespace UserService.App.Services;

public class UserActivityService : IUserActivityService
{
    private readonly KafkaProducerService _kafkaProducer;
    private readonly ILogger<UserActivityService> _logger;

    public UserActivityService(KafkaProducerService kafkaProducer, ILogger<UserActivityService> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }

    public async Task<bool> AddUserActivityAsync(UserActivityRequest request)
    {
         try
        {
            var message = JsonSerializer.Serialize(request);

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
        //TODO отправлять в сервис активностей
        return new List<UserActivityResponse>();
    }
}