using Microsoft.AspNetCore.Http;

namespace UserService.App.Models
{
    public class UserActivityRequest
    {
        public Guid? Id { get; set; }

        public Guid? UserId { get; set; } 

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string? Title { get; set; }

        public string? Text { get; set; }

        public virtual List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();
    }
}