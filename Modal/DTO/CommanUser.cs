using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Edu_Block_dev.DAL.EF;

namespace Edu_Block_dev.Modal.DTO
{
	public class CommanUser
	{
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [JsonIgnore]
        public string Password { get; set; }

        public bool Status { get; set; }
        public bool LoginStatus { get; set; }

        public UserProfile UserProfile { get; set; }

        public UserRole UserRole { get; set; }

        public string DID { get; set; }

        public string Mode { get; set; } = string.Empty;

        public RolesAndPermissionDTO RolesAndPermissionDTO { get; set; }

        public UniversityResponseDTO universityResponseDTO { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

