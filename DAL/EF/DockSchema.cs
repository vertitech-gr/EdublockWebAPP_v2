using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Edu_Block_dev.DAL.EF
{
    public class DockSchema
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string SchemaID { get; set; }

        public string Credentials { get; set; }

        [Required]
        [ForeignKey("UserProfile")]
        public Guid UserProfileId { get; set; }

        public string Properties { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool AdditionalProperties { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
