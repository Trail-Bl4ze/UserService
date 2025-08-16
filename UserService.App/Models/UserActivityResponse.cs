using Microsoft.AspNetCore.Http;

namespace UserService.App.Models
{
    public class UserActivityResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int? LikesCount { get; set; }
        public int? CommentsCount { get; set; }
        public List<string> ImagesUrls { get; set; }
    }
}