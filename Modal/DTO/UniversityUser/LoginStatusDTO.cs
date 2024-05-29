using System.ComponentModel.DataAnnotations;

namespace Edu_Block_dev.Modal.DTO
{
    public class LoginStatusDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public Guid RoleId { get; set; }
    }
}