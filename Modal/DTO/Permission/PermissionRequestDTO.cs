using System.ComponentModel.DataAnnotations;

namespace Edu_Block_dev.Modal.DTO
{
    public class PermissionRequestDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(100, ErrorMessage = "Description cannot be longer than 100 characters")]
        public string Description { get; set; }
    }
}