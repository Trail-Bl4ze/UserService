using UserService.App.Models;

namespace UserService.App.BackgroundJobs;
public class UserActivityKafkaMessage : UserActivityRequest
{
    public new Guid UserId { get; set; }
    public new List<string> ImageFiles { get; set; }
}