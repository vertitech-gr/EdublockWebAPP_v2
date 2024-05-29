using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Edu_Block_dev.DAL.EF
{
	public class UniversityDepartmentUser
	{

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey("Department")]
        [Required(ErrorMessage = "Department Id is required.")]
        public required Guid DepartmentId { get; set; }

        [ForeignKey("University")]
        [Required(ErrorMessage = "University Id is required.")]
        public required Guid UniversityId { get; set; }

        [ForeignKey("UniversityUser")]
        [Required(ErrorMessage = "University Id is required.")]
        public required Guid UniversityUserId { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

    }

}

