using System.ComponentModel.DataAnnotations;

namespace Edu_Block_dev.Modal.DTO
{
    public class UniversityUpdateRequestDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "CountryCode is required")]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }

        public string Type { get; set; }

        public bool Status { get; set; }

        public bool LoginStatus { get; set; }

        public bool Active { get; set; }
    }

}
