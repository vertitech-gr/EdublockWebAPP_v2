using System.ComponentModel.DataAnnotations;

namespace Edu_Block_dev.Modal.DTO
{
    public class DepartmentRequestDTO
    {
        public Guid DepartmentID { get; set; }

        public Guid UniversityID { get; set; }

        public string Email { get; set; }

        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "type is required")]
        [MaxLength(100, ErrorMessage = "Type cannot be longer than 100 characters")]
        public string Type { get; set; } = string.Empty;
    }
}