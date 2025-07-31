using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Domain.Entities
{

    [Table("user_activity", Schema = "user")]
    public class UserActivity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        [Required]
        public Guid UserId { get; set; }

        [Column("coordinate")]
        [Required]
        public PhotoCoordinate Coordinate { get; set; }

        [Column("title")]
        public string? Title { get; set; }

        [Column("text")]
        public string? Text { get; set; }

        [Column("image_urls")]
        [Required]
        public List<string> Images { get; set; }
    }

    public class PhotoCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}