using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edu_Block_dev.DAL.EF
{
	public class TemplateSchemaMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("Role")] // Assuming the related table is named "Role"
        public Guid TemplateId { get; set; }

        // Navigation property to represent the relationship with the role
        public virtual Template Template { get; set; }

        [Required]
        [ForeignKey("PermissionDetail")] // Assuming the related table is named "Role"
        public Guid SchemaId { get; set; }

        // Navigation property to represent the relationship with the role
        public virtual Schema Schema { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}

