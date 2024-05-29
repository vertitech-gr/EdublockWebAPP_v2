using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edu_Block_dev.DAL.EF
{
    public class RequestMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("UserProfile")]
        public Guid SenderId { get; set; }

        public Guid RequestId { get; set; }

        public string Description { get; set; }

        [Url(ErrorMessage = "Invalid URL format for Logo")]
        public string? Attachment { get; set; }

        public bool IsDeleted { get; set; } = false;

        public Edu_Block_dev.Authorization.Role Type { get; set; }

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
