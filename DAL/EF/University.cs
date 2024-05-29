using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Edu_Block.DAL.EF;
using System.Text.Json.Serialization;


public class University
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    public string Email { get; set; }

    public string Type { get; set; }

    public string CountryCode { get; set; }

    public string PhoneNumber { get; set; }

    public bool Status { get; set; }

    public bool Active { get; set; } = false;

    public bool loginStatus { get; set; } = true;

    public Guid universityId { get; set; }

    [Required]
    [JsonIgnore]
    public string Password { get; set; }

    [Required]
    public string Address { get; set; }

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

    public virtual UniversityDetail UniversityDetail { get; set; }

    [JsonIgnore]
    public virtual ICollection<Otp> Otps { get; set; }

}
