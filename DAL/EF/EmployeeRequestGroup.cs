using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Edu_Block_dev.DAL.EF
{
    public class EmployeeRequestGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("Request")]
        public Guid RequestId { get; set; }

        [Required]
        public Guid CertificateId { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

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
}
