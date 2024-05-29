using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Edu_Block_dev.Modal.DTO.EduSchema
{
	public class TemplateDTO
    {
        [Required(ErrorMessage = "Name is required")]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("html")]
        public string HTML { get; set; }

        [JsonProperty("css")]
        public string CSS { get; set; }

        [Required(ErrorMessage = "University is required")]
        public Guid UniversityId { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public Guid DepartmentId { get; set; }

        [Required(ErrorMessage = "Schema is required")]
        public Guid SchemaId { get; set; }
    }
}