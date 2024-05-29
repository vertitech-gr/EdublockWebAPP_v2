using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UniversityDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "UniversityId is required")]
    [ForeignKey("University")]
    public Guid UniversityId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; }

    public string? Description { get; set; }

    [Url(ErrorMessage = "Invalid URL format for Logo")]
    public string? Logo { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime? IsDeletedAt { get; set; } 

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}