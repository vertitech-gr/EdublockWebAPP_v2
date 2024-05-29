using System.ComponentModel.DataAnnotations;

namespace Edu_Block_dev.Modal.DTO.EduSchema
{
	public class SchemaDTO
	{
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(100, ErrorMessage = "Description cannot be longer than 100 characters")]
        public string Description { get; set; }

        public string Attributes { get; set; }

        [Required(ErrorMessage = "University is required")]
        public Guid UniversityId { get; set; }

    }

    public class Propeties
    {
        public string title { get; set; }
        public string type { get; set; }
        public string description { get; set; }
    }
}