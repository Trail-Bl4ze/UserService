namespace UserService.App.Models
{
    public class UserActivityDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public PhotoCoordinateDTO Coordinate { get; set; }

        public string? Title { get; set; }

        public string? Text { get; set; }

        public List<string> Images { get; set; }
    }

    public class PhotoCoordinateDTO
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}