using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edu_Block_dev.DAL.EF
{
	public class RolePermissionMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("Role")] // Assuming the related table is named "Role"
        public Guid RoleId { get; set; }

        // Navigation property to represent the relationship with the role
        public virtual Role Role { get; set; }

        [Required]
        [ForeignKey("PermissionDetail")] // Assuming the related table is named "Role"
        public Guid PermissionId { get; set; }

        // Navigation property to represent the relationship with the role
        public virtual PermissionDetail PermissionDetail { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}

