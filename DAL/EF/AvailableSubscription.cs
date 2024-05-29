using Edu_Block_dev.Modal.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edu_Block_dev.DAL.EF
{
    public class AvailableSubscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(256)]
        public SubscriptionType Type { get; set; }

        [Required]
        public decimal Coins { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Discription { get; set; }

        [Required]
        public bool is_deleted { get; set; }

        public DateTimeOffset? is_deleted_at { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        public Guid created_by { get; set; }

        public int updated_by { get; set; }
    }

}
