using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Domain.Entities
{
    [Table("user_profiles", Schema = "auth")]
    public class UserProfile
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        [Required]
        public Guid UserId { get; set; }

        [Column("first_name")]
        [Required]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [Column("last_name")]
        [Required]
        [MaxLength(50)]
        public string? LastName { get; set; }

        [Column("bio")]
        [MaxLength(500)]
        public string? Bio { get; set; }

        [Column("birth_date")]
        public DateTime? BirthDate { get; set; }

        [Column("gender")]
        [MaxLength(20)]
        public string? Gender { get; set; }

        [Column("profile_photo_url")]
        public string? ProfilePhotoUrl { get; set; }
    }
}