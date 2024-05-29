
using System.ComponentModel.DataAnnotations;


namespace Edu_Block_dev.Modal.DTO
{
    public class UpdateRoleRequestDTO
    {
        public Guid Guid { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        public string? ConcurrencyStamp { get; set; }

        public List<Guid> PermissionList { get; set; } = new List<Guid>();
    }
}