using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Edu_Block_dev.Modal.Enum;

namespace Edu_Block_dev.DAL.EF
{
    public class Envelope
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsDeleted { get; set; } = false;

        public EnvelopeShareType Type { get; set; }

        [Required]
        [ForeignKey("UserProfile")]
        public Guid UserProfileId { get; set; }

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

