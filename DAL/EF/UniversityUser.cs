using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edu_Block_dev.DAL.EF
{
	public class UniversityUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey("Role")]
        public Guid RoleId { get; set; }

        public virtual Role Role { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool Status { get; set; } = false;

        public bool Active { get; set; } = false;

        public bool loginStatus { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        [JsonIgnore]
        public string Password { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}

