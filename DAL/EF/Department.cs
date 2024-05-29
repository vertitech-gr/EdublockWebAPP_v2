using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Department
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Type is required")]
    [MaxLength(100, ErrorMessage = "Type cannot be longer than 100 characters")]
    public string Type { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    [JsonIgnore]
    public string Password { get; set; }

    public bool Status { get; set; }

    public bool loginStatus { get; set; } = true;


    [Required]
    [ForeignKey("University")]
    public Guid UniversityID { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime IsDeletedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid CreatedBy { get; set; }

    [Required]
    public Guid UpdatedBy { get; set; }


}
