
using System.ComponentModel.DataAnnotations;

namespace Edu_Block_dev.Modal.DTO
{
    public class UniversityUserRequestDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public Guid RoleId { get; set; }

        [Required(ErrorMessage = "University is required")]
        public Guid UniversityId { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public Guid DepartmentId { get; set; }
    }
}