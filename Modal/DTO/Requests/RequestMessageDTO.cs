
using System.ComponentModel.DataAnnotations;
using Edu_Block_dev.Authorization;

namespace Edu_Block_dev.Modal.DTO
{
    public class RequestMessageDTO
    {
        [Required(ErrorMessage = "PhoneNumber is required")]
        public Guid SenderId { get; set; }

        [Required(ErrorMessage = "CountryCode is required")]
        public Guid RequestId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public Role Type { get; set; }

        public IFormFile? Attachment { get; set; }
    }
}