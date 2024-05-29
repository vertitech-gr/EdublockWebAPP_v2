using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Edu_Block_dev.Modal.DTO;

namespace Edu_Block_dev.DAL.EF
{
    public class UserRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("UserProfile")]
        public Guid SenderId { get; set; }

        [Required]
        public Guid ReceiverId { get; set; }

        public string RequestType { get; set; }

        public MessageStatus Status { get; set; }

        public string Remark { get; set; }

        public string GraduationYear { get; set; }

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
}
