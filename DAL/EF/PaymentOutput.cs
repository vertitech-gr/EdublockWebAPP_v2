using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Edu_Block.DAL.EF
{
    public class PaymentOutput
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Account { get; set; }

        public string Action { get; set; }

        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        [JsonProperty("alt_key")]
        public string AltKey { get; set; }

        public string transaction { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}