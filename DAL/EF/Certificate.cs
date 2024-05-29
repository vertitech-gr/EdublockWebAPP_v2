using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Certificate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Issuer { get; set; }

    public string RegistryID { get; set; }

    public string CertificateID { get; set; }

    public string DegreeType { get; set; }

    public string DegreeName { get; set; }

    public string FileName { get; set; }

    public string Path { get; set; }

    public DateTime DegreeAwardedDate { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime IssuanceDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [JsonIgnore]
    public string Password { get; set; }

    public int CertificateTemplateId { get; set; }

    [Required]
    [ForeignKey("UserProfile")]
    public Guid UserProfileId { get; set; }

    public string CredentialsJson { get; set; }

    public bool IsDeleted { get; set; } = false;

    public bool Status { get; set; } = true;

    public DateTime IsDeletedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }

    [Required]
    public Guid CreatedBy { get; set; }

    [Required]
    public Guid UpdatedBy { get; set; }
}