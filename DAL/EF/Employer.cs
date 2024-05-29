using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Edu_Block_dev.DAL.EF
{
    public class Employer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();

        //[Required]
        //[ForeignKey("UserProfile")]
        //public Guid UserProfileId { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public bool Status { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string CountryCode { get; set; }

        public string PhoneNumber { get; set; }

       

        public string Address { get; set; }

        public string Industry { get; set; }

        public bool loginStatus { get; set; } = true;

        public string SpecificIndustry { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime IsDeletedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
