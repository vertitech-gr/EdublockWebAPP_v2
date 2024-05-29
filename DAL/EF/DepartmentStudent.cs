using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edu_Block.DAL.EF
{
    public class DepartmentStudent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("User")]
        [Required(ErrorMessage = "Student Id is required.")]
        public required Guid StudentId { get; set; }

        [ForeignKey("Department")]
        [Required(ErrorMessage = "Department Id is required.")]
        public required Guid DepartmentId { get; set; }

        [ForeignKey("University")]
        [Required(ErrorMessage = "University Id is required.")]
        public required Guid UniversityId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime IsDeletedAt { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        public Guid UpdatedBy { get; set; }
    }
}