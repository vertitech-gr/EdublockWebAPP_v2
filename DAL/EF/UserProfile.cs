using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserProfile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid UserID { get; set; }

    public string? Description { get; set; }

    public string? Picture { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime? IsDeletedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid CreatedBy { get; set; }

    [Required]
    public Guid UpdatedBy { get; set; }
}
