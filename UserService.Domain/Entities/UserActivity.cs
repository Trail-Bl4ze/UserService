using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Domain.Entities
{

    [Table("user_activities", Schema = "account")]
    public class UserActivity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        [Required]
        public Guid UserId { get; set; }

        [Column("title")]
        public string? Title { get; set; }

        [Column("text")]
        public string? Text { get; set; }

        [Column("image_urls")]
        [Required]
        public List<string> Images { get; set; }

        [Column("latitude")]
        [Required]
        public double Latitude { get; set; }

        [Column("longitude")]
        [Required]
        public double Longitude { get; set; }
    }
}