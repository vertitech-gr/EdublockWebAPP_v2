using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Edu_Block_dev.DAL.EF
{
    public enum ShareType
    {
        Certificate = 1,
        Envelope = 2
    }
    public class Share
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } 
        
        [Required]
        public Guid ResourceId { get; set; }

        public Guid RequsetId { get; set; }

        [ForeignKey("UserProfile")]
        public Guid SenderId { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public bool Verified { get; set; } = false;

        [Required]
        public int Count { get; set; } = 0;

        [Required]
        public string Token { get; set; }

        [Required]
        [EnumDataType(typeof(ShareType))]
        public ShareType Type { get; set; }

        [Required]
        public DateTime? ExpiryDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime IsDeletedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        public Guid UpdatedBy { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type != ShareType.Certificate && Type != ShareType.Envelope)
            {
                yield return new ValidationResult("Invalid ShareType value. Allowed values are 1 or 2.", new[] { nameof(Type) });
            }
        }
    }
}
