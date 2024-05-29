using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Edu_Block_dev.Modal.DTO;

namespace Edu_Block_dev.DAL.EF
{
    public class Transactions
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public TransactionType Type { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; } //

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime TransactionDate { get; set; }
        public TransactionStatus Status { get; set; }
        [Required]
        public Guid RefrenceId { get; set; } //
        [Required]
        public Guid UserProfileID { get; set; }

        [Required]
        public int PaymentID { get; set; } //
        public bool is_deleted { get; set; }
        public DateTimeOffset? is_deleted_at { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int created_by { get; set; }
        public int updated_by { get; set; }
    }
}
