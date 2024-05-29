using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edu_Block_dev.DAL.EF
{
    public class PurchaseSubscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid AvailableSubscriptionID { get; set; } = Guid.NewGuid();
        [Required]
        public decimal CoinBalance { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
       
        [Required]
        public Guid UserProfileID { get; set; }

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
