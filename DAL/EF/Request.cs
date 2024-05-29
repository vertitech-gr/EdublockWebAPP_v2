using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Edu_Block_dev.Modal.DTO;

namespace Edu_Block_dev.DAL.EF
{
    public class Request
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("UserProfile")]
        public Guid SenderId { get; set; }

        [Required]
        public Guid ReceiverId { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public string Discription { get; set; }

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
