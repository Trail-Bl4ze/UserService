using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Domain.Entities;

[Table("relationships", Schema = "account")]
public class Relationship 
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("follower_id")]
    public Guid FollowerId { get; set; } // Кто подписан

    [Column("following_id")]
    public Guid FollowingId { get; set; } // На кого подписан

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual User Follower { get; set; }
    public virtual User Following { get; set; }
}