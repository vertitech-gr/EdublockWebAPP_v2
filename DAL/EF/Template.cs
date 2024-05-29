using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edu_Block_dev.DAL.EF
{
	public class Template
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [JsonProperty("html")]
        public string Html { get; set; }

        [Required]
        [JsonProperty("css")]
        public string CSS { get; set; }

        [Required]
        public string TemplateId { get; set; }

        [ForeignKey("University")]
        [Required(ErrorMessage = "University Id is required.")]
        public required Guid UniversityId { get; set; }

        [ForeignKey("Department")]
        [Required(ErrorMessage = "Department Id is required.")]
        public required Guid DepartmentId { get; set; }

        [ForeignKey("Schema")]
        [Required(ErrorMessage = "Schema Id is required.")]
        public required Guid SchemaId { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}