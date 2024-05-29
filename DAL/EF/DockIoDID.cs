using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DockIoDID
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string DID { get; set; }

    public string Password { get; set; }

    public string Credentials { get; set; }

    [Required]
    [ForeignKey("UserProfile")]
    public Guid UserProfileId { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime IsDeletedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
