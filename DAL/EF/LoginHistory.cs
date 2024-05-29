using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edu_Block.DAL.EF
{
    public class LoginHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "User profile id is required for updating login history.")]
        [ForeignKey("UserProfile")]
        public required string UserProfileID { get; set; }

        public string? Device { get; set; }

        public string? CountryCode { get; set; }

        public string? CountryName { get; set; }

        public string? City { get; set; }

        public string? Os { get; set; }

        public string? Platform { get; set; }

        public string? Ip { get; set; }

        public string? Status { get; set; }

        public bool IsDeleted { get; set; }

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
}